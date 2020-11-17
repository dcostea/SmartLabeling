document.addEventListener('DOMContentLoaded', (event) => {

    document.querySelector("#start").onclick = function () {
        connection.invoke("StartSensorsStreaming");
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/sensorshub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("sensorsStreamingStarted", function () {
        console.log("SENSORS STREAMING STARTED");

        connection.stream("SensorsCaptureLoop").subscribe({
            close: false,
            next: data => {
                console.log("populating sensors data...");
                populateData(data);
            },
            err: err => {
                console.log(err);
            },
            complete: () => {
                console.log("finished sensors streaming");
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