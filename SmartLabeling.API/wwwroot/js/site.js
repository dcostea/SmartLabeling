var isInceptionTrained = false;

document.addEventListener('DOMContentLoaded', (event) => {

    const connection = new signalR.HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Information)
        .withUrl("http://192.168.178.21:5050/camerahub")
        .build();

    connection.on("streamingStarted", async function () {
        console.log("STREAMING STARTED");
        connection.stream("SensorsTick").subscribe({
            close: false,
            next: data => {
                console.log("populating data...");
                populateData(data);
            },
            err: err => {
                console.log(err);
            },
            complete: () => {
                console.log("finished streaming");
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

function startInceptionTraining() {

    var url = "main/train_inception";

    fetch(url, {
        method: 'GET',
        mode: 'cors'
    })
        .then((response) => {
            isInceptionTrained = true;
        })
        .catch(function () {
            //document.querySelector("#inception_source").innerHTML("inception training failure");
        });
}

function getPredictionByImage(image) {

    let url = 'main/predict_image';

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
