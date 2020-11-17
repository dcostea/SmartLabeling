var isInceptionTrained = false;
var fakeUrl;
var fakeCameraHub;
var fakeSensorsHub;
var capture = {};
var sensors = {};

document.addEventListener('DOMContentLoaded', async (event) => {

    //TODO intercept stop streaming and create a connection.on 

    // initialize
    document.querySelector("#stop").style.display = "none";

    await getSettings().then((response) => response.json())
        .then(function (data) {
            fakeUrl = data.fakeUrl;
            fakeCameraHub = data.fakeCameraHub;
            fakeSensorsHub = data.fakeSensorsHub;
        })
        .catch(function (err) {
            console.log(err.message);
        });

    ////////////////////// CAMERA /////////////////////////////
    const cameraConnection = new signalR.HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Information)
        .withUrl(fakeUrl + fakeCameraHub)
        .build();

    cameraConnection.on("cameraStreamingStarted", function () {
        console.log("FAKE CAMERA STREAMING STARTED");
        cameraConnection.stream("CameraCaptureLoop").subscribe({
            close: false,
            next: data => {
                populateCameraData(data);
            },
            err: err => {
                console.log(err);
            },
            complete: () => {
                console.log("finished fake camera streaming");
            }
        });
    });

    cameraConnection.on("cameraStreamingStopped", function () {
        console.log("FAKE CAMERA STREAMING STOPPED");
    });

    cameraConnection.start();

    ////////////////////// SENSORS /////////////////////////////
    const sensorsConnection = new signalR.HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Information)
        .withUrl(fakeUrl + fakeSensorsHub)
        .build();

    sensorsConnection.on("sensorsStreamingStarted", function () {
        console.log("FAKE SENSORS STREAMING STARTED");
        sensorsConnection.stream("SensorsCaptureLoop").subscribe({
            close: false,
            next: data => {
                populateSensorsData(data);
            },
            err: err => {
                console.log(err);
            },
            complete: () => {
                console.log("finished fake sensors streaming");
            }
        });
    });

    sensorsConnection.on("sensorsStreamingStopped", function () {
        console.log("FAKE SENSORS STREAMING STOPPED");
    });

    sensorsConnection.start();

    ////// EVENTS ///////////////////////////////////////////////

    document.querySelector("#start").onclick = function () {
        cameraConnection.invoke("StartCameraStreaming");
        sensorsConnection.invoke("StartSensorsStreaming");

        document.querySelector("#start").style.display = "none";
        document.querySelector("#stop").style.display = "block";
        document.querySelector("#save_csv").disabled = true;
    }

    document.querySelector("#stop").onclick = function () {
        cameraConnection.invoke("StopCameraStreaming");
        sensorsConnection.invoke("StopSensorsStreaming");

        document.querySelector("#start").style.display = "block";
        document.querySelector("#stop").style.display = "none";
        document.querySelector("#save_csv").disabled = false;
    }

    document.querySelector("#inception_train").onclick = function () {
        startInceptionTraining();
        document.querySelector("#inception_train").style.display = "none";
    }

    document.querySelector("#clear_list").onclick = function () {
        clearList();
    }

    document.querySelector("#save_csv").onclick = function () {
        saveCsv();
    }
})

function populateCameraData(data) {
    if (data !== undefined) {
        if (data.image !== undefined) {
            document.querySelector("#camera").setAttribute("src", `data:image/jpg;base64,${data.image}`);
            capture.createdAt = data.createdAt;
            getPredictionByImage(data.image);
        }
    }
}

function populateSensorsData(data) {
    if (data !== undefined) {
        if (data.luminosity !== undefined) {
            sensors.luminosity = data.luminosity;
            document.querySelector("#lux").innerHTML = `${data.luminosity} %`;
        }
        if (data.temperature !== undefined) {
            sensors.temperature = data.temperature;
            document.querySelector("#temp").innerHTML = `${data.temperature} &deg;C`;
        }
        if (data.infrared !== undefined) {
            sensors.infrared = data.infrared;
            document.querySelector("#infra").innerHTML = `${data.infrared} %`;
        }
        if (data.createdAt !== undefined) {
            sensors.createdAt = data.createdAt;
        }
    }
}

async function getSettings() {

    var url = "main/settings";

    var result = fetch(url, {
        method: 'GET',
        mode: 'cors'
    })

    return result;
}

function startInceptionTraining() {

    var url = "main/train_inception";

    fetch(url, {
        method: 'GET',
        mode: 'cors'
    })
        .then((response) => {
            isInceptionTrained = true;
        })
        .catch(function (err) {
            console.log(err.message);
        });
}

function getPredictionByImage(image) {

    let url = 'main/predict_image';

    document.querySelector("#prediction").innerHTML = "?";

    fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/octet-stream'
        },
        body: image
    })
        .then((response) => response.json())
        .then(function (data) {
            if (data !== undefined) {
                console.log(data);
                capture.source = data;
                document.querySelector("#prediction").innerHTML = data;
            }
            populateList(sensors, capture);
        })
        .catch(function (err) {
            console.log(err.message);
        });
}

function populateList(sensors, capture) {

    var tolerance = 200;
    var difference = Math.abs(capture.createdAt - sensors.createdAt);
    var isNear = difference < tolerance;
    var backColor = isNear ? "bg-warning" : "bg-danger";
    var checked = isNear ? "checked" : "";

    if (sensors.temperature !== undefined
        && sensors.luminosity !== undefined
        && sensors.infrared !== undefined
        && capture.source !== undefined
        && capture.createdAt !== undefined) {

        document.querySelector("#readings tbody").innerHTML = `<tr><td>${sensors.temperature}</td><td>${sensors.luminosity}</td><td>${sensors.infrared}</td><td title='${difference}' class='${backColor}'>${sensors.createdAt}</td><td title='${difference}' class='${backColor}'>${capture.createdAt}</td><td>${capture.source}</td><td><input type='checkbox' ${checked} /></td></tr>`
            + document.querySelector("#readings tbody").innerHTML;
    }
}

function clearList() {
    document.querySelector("#readings tbody").innerHTML = "";
}

function tableToJson(table) {
    var data = [];
    for (var i = 1; i < table.rows.length; i++) {
        var tableRow = table.rows[i];
        if (tableRow.cells[6].innerHTML.includes(" checked")) {
            var rowData = {
                temperature: tableRow.cells[0].innerHTML,
                luminosity: tableRow.cells[1].innerHTML,
                infrared: tableRow.cells[2].innerHTML,
                createdAt: tableRow.cells[3].innerHTML,
                source: tableRow.cells[5].innerHTML,
            };
            data.push(rowData);
        }
    }
    return data;
}

function saveCsv()
{
    let url = 'main/save_csv';
    var list = tableToJson(document.querySelector("#readings"))

    fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(list)
    })
        .then((response) => response.json())
        .then(function (data) {
            document.querySelector("#readings").innerHTML = "";
        })
        .catch(function (err) {
            console.log(err.message);
        });
}