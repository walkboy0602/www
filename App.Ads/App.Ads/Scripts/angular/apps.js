
//declare a module
var adsApp = angular.module("adsApp", ['adsAPI', 'ui.bootstrap', 'ngRoute', 'ngSanitize', 'ngResource', 'mgcrea.ngStrap', 'cgBusy'])
                    .config(function ($locationProvider, $routeProvider) {
                        $routeProvider.when('/listing/image/:id', {
                            controller: ListingImageCtrl
                        });

                    });

adsApp.run(['$rootScope', '$window', '$http', '$location', '$route', '$routeParams',
    function ($rootScope, $window, $http, $location, $route, $routeParams) {
        //add this so server side will recognise as ajax request
        $http.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';

        $rootScope.Enum = {
            ReferenceType: {
                None: 0,
                SaleType: 1,
                Category: 2,
                Tag: 3
            }
        }


        //set global data
        if (undefined !== $window.data) {
            $rootScope.id = $window.data.id;
        }

        $rootScope.errorMessage = "Sorry, there was an error while processing your request. <br/> Please contact our support.";

        $rootScope.success = function (message, isHide) {
            $rootScope.alert = {
                type: 'success',
                display: true,
                message: message,
                showGobal: angular.isUndefined(isHide) ? true : isHide
            };
        }

        $rootScope.info = function (message, isHide) {
            $rootScope.alert = {
                type: 'info',
                display: true,
                message: message,
                showGobal: angular.isUndefined(isHide) ? true : isHide
            };
        }

        $rootScope.warning = function (message, isHide) {
            $rootScope.alert = {
                type: 'warning',
                display: true,
                message: message,
                showGobal: angular.isUndefined(isHide) ? true : isHide
            };
        }

        $rootScope.error = function (message, isHide) {
            $rootScope.alert = {
                type: 'danger',
                display: true,
                message: angular.isUndefined(message) ? $rootScope.errorMessage : message,
                showGobal: angular.isUndefined(isHide) ? true : isHide
            };
        }

        //Spin
        $rootScope.spinLoading = {
            opts: {
                lines: 9, // The number of lines to draw
                length: 11, // The length of each line
                width: 7, // The line thickness
                radius: 19, // The radius of the inner circle
                corners: 0.4, // Corner roundness (0..1)
                rotate: 80, // The rotation offset
                direction: 1, // 1: clockwise, -1: counterclockwise
                color: '#000', // #rgb or #rrggbb or array of colors
                speed: 1, // Rounds per second
                trail: 58, // Afterglow percentage
                shadow: false, // Whether to render a shadow
                hwaccel: false, // Whether to use hardware acceleration
                className: 'spinner', // The CSS class to assign to the spinner
                zIndex: 2e9, // The z-index (defaults to 2000000000)
                top: '50%', // Top position relative to parent in px
                left: '50%' // Left position relative to parent in px
            },
            start: function (index) {

                var target = $("div.spin-" + index)[0];
                //var target3 = $("div[class*=spin-")[0];

                if (!target) {
                    return;
                }

                console.log(target);
                this.spinner = new Spinner(this.opts).spin(target);


            },
            end: function () {
                this.spinner.stop();
            }
        };


    }]);
