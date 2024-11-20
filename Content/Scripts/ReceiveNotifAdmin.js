$(document).ready(function () {
    // Toggle notification dropdown on bell click with a slow slide
    $('#notificationBell').click(function (e) {
        e.preventDefault();
        $('#notificationDropdown').slideToggle('slow'); // Slide toggle with slow effect
    });

    // Close the dropdown if clicking outside of it
    $(document).click(function (e) {
        var target = $(e.target);
        if (!target.closest('#notificationBell').length && !target.closest('#notificationDropdown').length) {
            $('#notificationDropdown').slideUp('slow'); // Close with slow effect
        }
    });
});
