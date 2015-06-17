

function loadTemplates(templateType, templateName, callbackFn) {    
    switch (templateType) {
        case "get_user_template_list":
        case "get_system_template_list":
            $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading List of Templates...</p>' });
            $.ajax({
                type: "POST",
                url: "RequestHandler.ashx",
                data: JSON.stringify({ Instruction: templateType, CommunicationType: communicationsMode }),
                dataType: "json",
            }).done(function (data) {
                $.unblockUI();
                callbackFn(data);
            });
            break;
        case "get_newsletter":
            $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading Newsletter...</p>' });
            // http://www.seeff.com/newsletters/may_2015/index.html
            var url = 'http://www.seeff.com/newsletters/' + getMonthName() + '_' + new Date().getFullYear() + '/index.html';
            $.ajax({
                type: "POST",
                url: "RequestHandler.ashx",
                data: JSON.stringify({ Instruction: 'get_template', IsFromUrl: true, URL: url }),
                dataType: "json"
            }).done(function (data) {
                $.unblockUI();
                callbackFn(data);
            });
            break;
        case "get_user_template":
            $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading Template...</p>' });
            $.ajax({
                type: "POST",
                url: "RequestHandler.ashx",
                data: JSON.stringify({ Instruction: "get_template", CommunicationType: communicationsMode, TemplateName: templateName, IsSystemTemplate: false }),
                dataType: "json"
            }).done(function (data) {
                $.unblockUI();
                callbackFn(data);
            });
            break;
        case "get_system_template":
            $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Loading Template...</p>' });
            $.ajax({
                type: "POST",
                url: "RequestHandler.ashx",
                data: JSON.stringify({ Instruction: "get_template", CommunicationType: communicationsMode, TemplateName: templateName, IsSystemTemplate: true }),
                dataType: "json"
            }).done(function (data) {
                $.unblockUI();
                callbackFn(data);
            });
            break;
    }
}

function saveUserTemplate() {
    if (communicationsMode == "EMAIL") {
        var emailSubject = $('#emailSubject').val().trim();
        var emailBody = $('#emailMessageBody').val().trim();
        emailBody = encodeURIComponent(btoa(emailBody));
        $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Saving Template...</p>' });
        $.ajax({
            type: "POST",
            url: "RequestHandler.ashx",
            data: JSON.stringify({ Instruction: 'add_update_template', CommunicationType: communicationsMode, TemplateName: emailSubject, TemplateContent: emailBody }),
            dataType: "json",
        }).done(function (data) {
            $.unblockUI();
            showSavedSplashDialog('Template saved!');
        });
    }
    // Implement for SMS
}

function deleteTemplate(templateName, callbackFn) {
    $.blockUI({ message: '<p style="font-family:Verdana;font-size:15px;">Deleting Template...</p>' });
    $.ajax({
        type: "POST",
        url: "RequestHandler.ashx",
        data: JSON.stringify({ Instruction: 'delete_template', CommunicationType: communicationsMode, TemplateName: templateName }),
        dataType: "json"
    }).done(function (data) {
        $.unblockUI();
        showSavedSplashDialog('Template Deleted!');
        callbackFn();
    });
}