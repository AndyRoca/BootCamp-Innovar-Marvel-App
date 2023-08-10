var modal = window.modal || {};
(function ()
{
    this.buttonOnClick = function ()
    {
        console.log("Respondio ")
        var pageInput = {
            pageType: "webresource",
            webresourceName: "arc_AddProductModal",
        };
        var navigationOptions = {
            target: 2,
            height: { value: 30, unit: "%" },
            width: { value: 30, unit: "%" },
            position: 1,
        };
        Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(
            function success()
            {
                // Run code on success
                console.log("Loaded");
            },
            function error()
            {
                // Handle errors
            }
        );
    };
}).call(modal);

function CallCustomActionFromJavaScript()
{

    let globalContext = parent.Xrm.Utility.getGlobalContext();

    let serverUrl = globalContext.getClientUrl();

    let actionName = "arc_MyCustomAction";

    let InputParamValue = document.getElementById("comicId").value;

    let data = {
        MyInputParam: InputParamValue,
    };

    let request = new XMLHttpRequest();
    request.open("POST", `${serverUrl}/api/data/v9.2/${actionName}`, true);
    request.setRequestHeader("Accept", "application/json");
    request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    request.setRequestHeader("OData-MaxVersion", "4.0");
    request.setRequestHeader("OData-Version", "4.0");

    request.onreadystatechange = function ()
    {
        if (this.readyState === 4)
        {
            request.onreadystatechange = null;

            if (this.status === 200 || this.status === 204)
            {

                console.log("Action Called Successfully...");

                result = JSON.parse(this.response);

                console.log(result);

            } else
            {
                var error = JSON.parse(this.response).error;
                console.log("Error in Action: " + error.message);
            }
        }
    };
    request.send(window.JSON.stringify(data));
}