$(document).ready(function () {
    // Function to fetch notifications
    function fetchNotifications() {
        $.ajax({
            url: '/Notification/FetchNotifications', // Endpoint to fetch notifications
            type: 'GET',
            success: function (notifications) {
                $('#notification-counter').text(notifications.length);
                $('#notification-list').empty(); // Clear previous notifications
                notifications.forEach(function (notification) {
                    var imageUrl = notification.Sender?.ProfileImage != null
                        ? `data:image/*;base64,${notification.Sender.ProfileImage}`
                        : '~/img/undraw_profile.svg';
                    var senderName = `${notification.Sender?.FirstName || ''} ${notification.Sender?.LastName || ''}`;
                    var timestamp = notification.Timestamp;

                    var notificationHtml = `
                        <a class="dropdown-item d-flex align-items-center" href="#">
                            <div class="dropdown-list-image mr-3">
                                <img class="rounded-circle" src="${imageUrl}" alt="...">
                            </div>
                            <div>
                                <div class="text-truncate">${notification.Message}</div>
                                <div class="small text-gray-500">${senderName}</div>
                                <div class="small text-gray-500">${timestamp}</div>
                            </div>
                        </a>
                    `;
                    $('#notification-list').append(notificationHtml);
                });
            },
            error: function (error) {
                console.log('Error fetching notifications:', error);
            }
        });
    }

    // Fetch notifications when page loads
    fetchNotifications();
});
