document.addEventListener('DOMContentLoaded', (event) => {

    document.querySelector("#start").onclick = function () {
        connection.invoke("StartStreaming");
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/camerahub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("streamingStarted", function () {
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
})

function populateData(data) {
    if (data !== undefined) {
        if (data.image !== undefined) {
            document.querySelector("#camera").setAttribute("src", `data:image/jpg;base64,${data.image}`);
        }
    }
}

