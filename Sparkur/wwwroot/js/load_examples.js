"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/initializerHub").build();
var progress = 0;

function addListItem(text, list) {
    var li = document.createElement("li");
    li.textContent = text;
    document.getElementById(list).appendChild(li);
}

connection.on("UpdateProgress", function (message, progress) {

    // var output = $("#resultmessage");
    var n = parseInt(message.Progress);

    addListItem("Status: <b>" + n + "%</b> " + message.Message + ' ' + progress, "messagesList");

    //var percetage = $("#percentage");
    //percetage.html(message.Progress + "%");

    //$("#pbar").css("width", n + "%").attr('aria-valuenow', n);

    if (n === 100)
    {
        // find progress bar and .removeClass("progress-bar-striped");
    }

}); 

connection.on("Pong", function (message) {
    var encodedMsg = "Server says " + message;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
    addListItem("test", "messagesList");
});

connection.start().then(function(){
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = "test";
    connection.invoke("LoadData").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});


// ----------

$(document).ready(function ()
{
    // initialize progress bar
    var progress = 0;
    $("#pbar").css("width", progress + "%").attr('aria-valuenow', progress);

    // initialize the connection to the server
    var progressNotifier = $.connection.initializerHub;

    // client-side sendMessage function that will be called from the server-side
    progressNotifier.client.sendMessage =
        function (message)
        {
            UpdateProgress(message);
        };

    // establish the connection to the server and start server-side operation
    $.connection.hub.start().done(
        function ()
        {
            progressNotifier.server.loadData();
        });

});

function UpdateProgress(message)
{

}