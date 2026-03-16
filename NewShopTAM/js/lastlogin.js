function checklogin(userid, id) {
    $.ajax({
        url: '@Url.Action("GetdataSessionlogin", "DataCenter")',
        data: {
            UsrID: userid,
            SessionId: id

        },
        type: "POST",
        dataType: "JSON",
        success: function (data) {
            var splitstring = data.StrStstuslogin;
            console.log(splitstring);
            let text = splitstring;
            const myArray = text.split("|");
            let mysession = myArray[1];
            let mystatus = myArray[0];
            console.log("mystatus " + mystatus + "//////" + "mysession " + mysession);
            if (id != mysession) {
                window.location.replace("../Account/LogIn");
            } else {

                if (mystatus == "0") {
                    window.location.replace("../Account/LogIn");
                }
            }
        }
    });

}