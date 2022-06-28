var app = angular.module('myApp', ['ui.calendar']);
app.controller('myNgController', ['$scope', '$http', 'uiCalendarConfig', function ($scope, $http, uiCalendarConfig) {

    $scope.SelectedEvent = null;
    var isFirstTime = true;

    $scope.events = [];
    $scope.eventSources = [$scope.events];

    var c = '';
    var label = "";

    $http.get('/dashboard/getlist', {
        cache: true,
        params: {}
    }).then(function (data) {
        $scope.events.slice(0, $scope.events.length);
        angular.forEach(data.data, function (value) {
            if (value.IsDone == 0) {
                label = "Do Checklist";
                c = 'red';
            }
            else
                if (value.IsDone == 0) {
                    label = " Partially Complete";
                    c = 'orange';
                }
                else
                    if (value.IsDone == 0) {
                        label = "Completed";
                        c = 'green';
                    }

            $scope.events.push({
                title: label,
                stick: false,
                color: c,
            });
        });
    });

    $scope.uiConfig = {
        calendar: {
            height: 'auto',
            //editable: true,
            displayEventTime: false,
            header: {
                left: 'month basicDay basicWeek',
                center: 'title',
                right: 'today prev,next'
            },
            eventClick: function (event) {
                $scope.SelectedEvent = event;
            },

            eventAfterAllRender: function () {
                if ($scope.events.length > 0 && isFirstTime) {

                    uiCalendarConfig.calendars.myCalendar.fullCalendar('gotoDate', $scope.events[0].start);
                    isFirstTime = false;
                }
            }
        }
    };

}])
//basicDay agendaWeek agendaDay