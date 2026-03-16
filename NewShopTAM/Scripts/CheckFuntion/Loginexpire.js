function expire(usrexpire) {
    var expireval;
    expireval = usrexpire;
    var _typ = usertype;
    var x = expireval.replace("Passwords expire '", "");
    expireval = x.replace("' days", "");

    if (parseInt(expireval) <= 15) {
        $("#textAlertWarning").empty();
        if (_typ == "2") {
            // $("#textAlertWarning").text("Password ของท่านอีก" + usrexpire + "วัน ใกล้หมดอายุ กรุณาเปลี่ยน Password  เข้าไปเปลี่ยน Password ได้ที่ App Citrix");
            alert("Password ของท่านอีก" + usrexpire + "วัน ใกล้หมดอายุ กรุณาเปลี่ยน Password  เข้าไปเปลี่ยน Password ได้ที่ App Citrix");
        } else if (_typ == "1") {
            // $("#textAlertWarning").text("Password ของท่านอีก " + usrexpire + "วัน ใกล้หมดอายุ กรุณาเปลี่ยน Password ");
            alert("Password ของท่านอีก " + usrexpire + "วัน ใกล้หมดอายุ กรุณาเปลี่ยน Password ");
        }
       // $("#myModalWarning").modal();
       
    }
}