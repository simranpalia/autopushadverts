
function pullFBGroups(token, userId, callbackUrl) {
    FB.api(
        "/" + userId + "/groups", { access_token: token },
        function (response) {
            debugger;
            if (response && !response.error) {
                $.each(response.data, function (indx, el) {

                    debugger;

                    $.ajax({
                        url: '/Test/UpdateFBGroups',
                        type: 'POST',
                        data: { groupId: el.id, gName: el.name, fbUserId: userId },
                        success: function (resp) {
                            debugger;
                            if (indx == (response.data.length - 1)) {
                                window.location.href = callbackUrl;
                            }
                        }
                    });
                });
            }
        }
    );

}