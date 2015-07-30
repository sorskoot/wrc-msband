// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=329104
(function () {
    "use strict";
    window.$ = function (query) { return document.querySelector(query); }

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var accelx, accely, accelz;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                accelx = $("#accelX");
                accely = $("#accelY");
                accelz = $("#accelZ");

                var band = new TimmyTools.Band();
                band.onaccelerometerchanged = function (sensordata) {
                    accelx.innerText = sensordata.z;
                    accely.innerText = sensordata.y;
                    accelz.innerText = sensordata.z;
                };

                band.getPaired().then(function (bands) {
                    var x = bands;
                    band.connect(bands[0].name).then(function (y) {

                    }, function (err) {
                        console.log(err);
                    })

                });
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application was suspended and then terminated.
                // To create a smooth user experience, restore application state here so that it looks like the app never stopped running.
            }
            args.setPromise(WinJS.UI.processAll());
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    app.start();
})();
