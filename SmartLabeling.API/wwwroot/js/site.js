var isInceptionTrained = false;
var fakeUrl;
var fakeCameraHub;
var fakeSensorsHub;

document.addEventListener('DOMContentLoaded', async (event) => {

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

    cameraConnection.on("cameraStreamingStarted", async function () {
        console.log("FAKE CAMERA STREAMING STARTED");
        cameraConnection.stream("CameraCaptureLoop").subscribe({
            close: false,
            next: data => {
                console.log("populating fake camera data...");
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

    cameraConnection.start();

    ////////////////////// SENSORS /////////////////////////////
    const sensorsConnection = new signalR.HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Information)
        .withUrl(fakeUrl + fakeSensorsHub)
        .build();

    sensorsConnection.on("sensorsStreamingStarted", async function () {
        console.log("FAKE SENSORS STREAMING STARTED");
        sensorsConnection.stream("SensorsCaptureLoop").subscribe({
            close: false,
            next: data => {
                console.log("populating fake sensors data...");
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

    sensorsConnection.start();
    ////////////////////////////////////////////////////////////

    document.querySelector("#start").onclick = function () {
        cameraConnection.invoke("StartCameraStreaming");
        sensorsConnection.invoke("StartSensorsStreaming");
    }

    document.querySelector("#inception_train").onclick = function () {
        startInceptionTraining();
        document.querySelector("#inception_train").style.display = "none";
        //document.querySelector("#inception_source").style.display = "block";
    }
})

function populateCameraData(data) {
    if (data !== undefined) {
        if (data.image !== undefined) {
            document.querySelector("#camera").setAttribute("src", `data:image/jpg;base64,${data.image}`);
            getPredictionByImage(data.image);
        }
    }
}

function populateSensorsData(data) {
    if (data !== undefined) {
        if (data.luminosity !== undefined)
            document.querySelector("#lux").innerHTML = `${data.luminosity} %`;
        if (data.temperature !== undefined)
            document.querySelector("#temp").innerHTML = `${data.temperature} &deg;C`;
        if (data.infrared !== undefined)
            document.querySelector("#infra").innerHTML = `${data.infrared} %`;
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
                document.querySelector("#prediction").innerHTML = data;
            }
        });
}
