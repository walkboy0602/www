//Tutorial of how to use Angular integrate with Web Form
//http://weblogs.asp.net/dwahlin/archive/2013/08/16/using-an-angularjs-factory-to-interact-with-a-restful-service.aspx

angular.module('adsAPI', [])
    .factory('AccountFactory', ['$http', function ($http) {
        return {
            emailVerification: function () {
                return $http.post('/account/verification/sendemail');
            },
            changePassword: function (data) {
                return $http.post('/account/edit/changepassword', data);
            }
        }
    }])
    .factory('ReferenceFactory', ['$http', function ($http) {

        return {
            get: function (data) {
                return $http.get('/reference/', { params: { type: data } });
            }
        }
    }])
    .factory('AdsFactory', ['$http', function ($http) {
        return {
            get: function (PageSize) {
                return $http.get('/api/ads', { params: { PageSize: PageSize } });
            },
        }
    }])
    .factory('ImageFactory', ['$http', function ($http) {
        return {
            get: function (ListingId) {
                return $http.get('/api/image', { params: { ListingId: ListingId } });
            },
            put: function (data) {
                return $http.put('/api/image', data);
            },
            delete: function (ImageId) {
                return $http.post('/api/image/delete?ImageId=' + ImageId);
            }
        }
    }])
    .factory('EnquiryFactory', ['$http', function ($http) {
        return {
            post: function (data) {
                return $http.post('/api/enquiry', data);
            }
        }
    }])
    .factory('CommonFactory', ['$http', function ($http) {
        return {
            getArea: function (locationId) {
                return $http.get('/search/getArea', { params: { locationId: locationId } });
            },
            getCategory: function () {
                return $http.get('/api/category');
            },
            getReference: function (data) {
                return $http.get('/api/reference/' + data);
            }
        }
    }])
    .factory('ListingFactory', ['$http', function ($http) {

        return {
            create: function (data) {
                return $http.post('/listingv2/create', { param: data })
            }
        }
    }]);


