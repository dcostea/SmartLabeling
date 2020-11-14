document.addEventListener('DOMContentLoaded', (event) => {

    const connection = new signalR.HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Error)
        .withUrl("http://192.168.178.21:5555/sensorhub")
        .build();

    connection.on("streamingStarted", async function (data) {
        connection.stream("SensorsTick").subscribe({
            close: false,
            next: sensors => {
                console.log(data);
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
})