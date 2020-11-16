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

    const connection = new signalR.HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Information)
        .withUrl(fakeUrl + fakeCameraHub)
        .build();

    connection.on("streamingStarted", async function () {
        console.log("FAKE STREAMING STARTED");
        connection.stream("CameraCaptureLoop").subscribe({
            close: false,
            next: data => {
                console.log("populating fake data...");
                populateData(data);
            },
            err: err => {
                console.log(err);
            },
            complete: () => {
                console.log("finished fake streaming");
            }
        });
    });

    connection.start();


    document.querySelector("#start").onclick = function () {
        connection.invoke("StartStreaming");
    }

    document.querySelector("#inception_train").onclick = function () {
        startInceptionTraining();
        document.querySelector("#inception_train").style.display = "none";
        //document.querySelector("#inception_source").style.display = "block";
    }
})

function populateData(data) {
    if (data !== undefined) {
        if (data.image !== undefined) {
            document.querySelector("#camera").setAttribute("src", `data:image/jpg;base64,${data.image}`);
            getPredictionByImage(data.image);
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
                document.querySelector("#prediction").innerHTML = data;
            }
        });
}
