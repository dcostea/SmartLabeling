document.addEventListener('DOMContentLoaded', (event) => {

    document.querySelector("#start").onclick = function () {
        connection.invoke("StartSensorsStreaming");
    }

    const connection = new signalR.HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Information)
        .withUrl("/sensorshub")
        .build();

    connection.on("sensorsStreamingStarted", function () {
        console.log("SENSORS STREAMING STARTED");
        connection.stream("SensorsCaptureLoop").subscribe({
            close: false,
            next: data => {
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

    connection.on("sensorsStreamingStopped", function () {
        console.log("SENSORS STREAMING STOPPED");
    });

    connection.on("sensorsDataCaptured", function (data) {
        console.log(`sensors data captured, ${data}`);
    });

    connection.on("sensorsDataNotCaptured", function () {
        console.log(`sensors data not captured, IoT device error`);
    });

    connection.start();
})

function populateData(data) {
    if (data !== undefined) {
        if (data.luminosity !== undefined)
            document.querySelector("#lux").innerHTML = `${data.luminosity} %`;
        if (data.temperature !== undefined)
            document.querySelector("#temp").innerHTML = `${data.temperature} &deg;C`;
        if (data.infrared !== undefined)
            document.querySelector("#infra").innerHTML = `${data.infrared} %`;
    }
}