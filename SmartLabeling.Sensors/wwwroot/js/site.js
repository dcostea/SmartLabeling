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
})
