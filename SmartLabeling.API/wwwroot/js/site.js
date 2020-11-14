document.addEventListener('DOMContentLoaded', (event) => {

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/sensorhub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    async function start() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.log(err);
            setTimeout(start, 5000);
        }
    };

    connection.onclose(start);

    // Start the connection.
    start();








    ////const connection = new signalR.HubConnectionBuilder()
    ////    .configureLogging(signalR.LogLevel.Error)
    ////    .withUrl("http://192.168.178.21:5000/sensor")
    ////    .build();

    ////connection.on("streamingStarted", async function (source) {
    ////    console.log("STREAMING STARTED");

    ////    connection.stream("SensorsTick").subscribe({
    ////        close: false,
    ////        next: sensors => {

    ////            getPredictionByImage(sensors.image);
    ////            $("#camera").attr('src', "data:image/jpg;base64," + `${sensors.image}`);

    ////            if (isTrained) {
    ////                getPrediction(sensors);
    ////            }
    ////        },
    ////        err: err => {
    ////            console.log(err);
    ////        },
    ////        complete: () => {
    ////            console.log("finished streaming");
    ////        }
    ////    });
    ////});

    ////connection.start();
})