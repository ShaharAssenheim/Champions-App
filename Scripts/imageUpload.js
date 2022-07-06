
$(document).ready(function () {
    $(".input-file").before(
        function () {
            if (!$(this).prev().hasClass('input-ghost')) {
                var element = $("<input type='file' id='input-ghost' style='display: none;'>");
                element.attr("name", $(this).attr("name"));
                element.change(function () {
                    
                    var formData = new FormData();
                    var totalFiles = document.getElementById("input-ghost").files.length;
                    for (var i = 0; i < totalFiles; i++) {
                        var file = document.getElementById("input-ghost").files[i];
                        formData.append("imageInput", file);
                        break;
                    }
                   
                    $.ajax({
                        type: "POST",
                        url: '/Customer/UploadImage',
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        beforeSend: function () {
                            $("#spinner").show(); 
                        },
                        success: function (data) {
                            $("#imageInput").val(data.responseText)
                            $("#spinner").hide(); 
                        },
                        error: function (error) {

                            alert("error " + error.responseText);
                            $("#spinner").addClass('no-js');
                        }

                    });
                });
                $(this).find("button.btn-choose").click(function () {
                    element.click();
                });
                return element;
            }
        });

});

