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
                    $('#notification-list').append(`
                        <a class="dropdown-item d-flex align-items-center" href="#">
                            <div class="dropdown-list-image mr-3">
                                <img class="rounded-circle" src="${notification.imageUrl}" alt="...">
                            </div>
                            <div>
                                <div class="text-truncate">${notification.message}</div>
                                <div class="small text-gray-500">${notification.timestamp}</div>
                            </div>
                        </a>
                    `);
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
