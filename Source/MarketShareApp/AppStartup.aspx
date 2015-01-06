<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppStartup.aspx.cs" Inherits="AppStartup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

     <%-- Jquery --%>
    <script src="http://code.jquery.com/jquery-1.10.1.min.js"></script>

    <script>
        $(function () {

            var form = $('<form action="Default.aspx" method="post">' +
                        '<input type="hidden" name="api_url" value="" />' +
                        '</form>');
            $('body').append(form);
            $(form).submit();

            $('#loadgif').attr('src', 'Assets/loading.gif');
        });
    </script>
</head>
<body style="display:block; overflow:hidden;">
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
