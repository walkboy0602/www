//Tutorial of how to use Angular integrate with Web Form
//http://weblogs.asp.net/dwahlin/archive/2013/08/16/using-an-angularjs-factory-to-interact-with-a-restful-service.aspx

angular.module('shopAPI', [])
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
            list: function (data) {
                return $http.get('/reference/get', { params: { ReferenceType: data } });
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
            }
        }
    }])
    .factory('ListingFactory', ['$http', function ($http) {

        return {
            getImage: function (id) {
                return $http.get('/listing/getImage', { params: { id: id } });
            },
            editImage: function (data) {
                return $http.post('/listing/editImage', data);
            },
            deleteImage: function (data) {
                return $http.post('/listing/deleteImage', data);
            },
            getSpecification: function (id) {
                return $http.get('/listing/getSpecification', { params: { ListingID: id } });
            },
            saveSpecification: function (data) {
                return $http.post('/listing/saveSpecification', data);
            },
            saveDescription: function (data) {
                return $http.post('/listing/saveDescription', data);
            },
            getDescription: function (id) {
                return $http.get('/listing/getDescription', { params: { ListingID: id } });
            },
            saveOption: function (data) {
                return $http.post('/listing/saveOption', data);
            },
            getOption: function (id) {
                return $http.get('/listing/getOption', { params: { ListingID: id } });
            },
            editOption: function (data) {
                return $http.post('/listing/editOption', data);
            },
            deleteOption: function (data) {
                return $http.post('/listing/deleteOption', data);
            }
        }
    }]);


