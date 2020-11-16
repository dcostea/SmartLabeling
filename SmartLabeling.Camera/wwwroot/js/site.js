document.addEventListener('DOMContentLoaded', (event) => {

    document.querySelector("#start").onclick = function () {
        connection.invoke("StartCameraStreaming");
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/camerahub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("cameraStreamingStarted", function () {
        console.log("CAMERA STREAMING STARTED");

        connection.stream("CameraCaptureLoop").subscribe({
            close: false,
            next: data => {
                console.log("populating camera data...");
                populateData(data);
            },
            err: err => {
                console.log(err);
            },
            complete: () => {
                console.log("finished camera streaming");
            }
        });
    });

    connection.start();
})

function populateData(data) {
    if (data !== undefined) {
        if (data.image !== undefined) {
            document.querySelector("#camera").setAttribute("src", `data:image/jpg;base64,${data.image}`);
        }
    }
}

