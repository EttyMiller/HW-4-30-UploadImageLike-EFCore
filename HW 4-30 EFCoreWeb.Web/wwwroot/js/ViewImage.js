$(() => {

    const imageId = $('#image-id').val();

    setInterval(function () {
            updateLikes();
    }, 1000);

    $("#like-button").on('click', function () {
        $.post('/image/updatelikes', { imageId }, function () {
            updateLikes()
            $("#like-button").prop("disabled", true);
        })
    })

    function updateLikes() {
        $.get('/image/getlikes', { imageId }, function (likes) {
            $("#likes-count").text(likes)
        });
    }
})



