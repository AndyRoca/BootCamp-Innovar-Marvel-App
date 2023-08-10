function CallCustomActionFromJavaScript()
{

    let globalContext = parent.Xrm.Utility.getGlobalContext();

    let serverUrl = globalContext.getClientUrl();

    let actionName = "arc_MyCustomAction";

    let InputParamValue = globalContext.userSettings.userId;

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

                console.log(result.MyOutputParam);

            } else
            {
                var error = JSON.parse(this.response).error;
                console.log("Error in Action: " + error.message);
            }
        }
    };
    request.send(window.JSON.stringify(data));
}