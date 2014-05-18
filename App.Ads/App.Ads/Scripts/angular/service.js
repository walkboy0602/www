shopApp.service('uploadManager', ['$rootScope', function ($rootScope) {
    var _files = [];
    return {
        add: function (file) {
            _files.push(file);
            if (file.fileInput) {
                $rootScope.$broadcast('fileAdded', file.files[0], file.fileInput[0].id);
            } else {
                $rootScope.$broadcast('fileAdded', file.files[0], null);
            }
        },
        clear: function () {
            _files = [];
        },
        files: function () {
            var fileNames = [];
            $.each(_files, function (index, file) {
                fileNames.push(file.files[0].name);
            });
            return fileNames;
        },
        upload: function (data) {
            var cache = [];
            $.each(_files, function (index, file) {
                $rootScope.data = data;
                file.submit();
            });
            cache = _files;
            this.clear();
            return cache;
        },
        setProgress: function (data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $rootScope.$broadcast('uploadProgress', progress);
        },
        done: function (data) {
            $rootScope.$broadcast('uploadReturn', data);
        }
    };
}])

shopApp.service('ListingManager', ['$rootScope', '$q', 'ListingFactory', 'ReferenceFactory',
    function ($rootScope, $q, ListingFactory, ReferenceFactory) {

        //Get listing info
        this.formDefer = function () {
            var q = $q.defer();
            ListingFactory.get($rootScope.id)
                .then(function (response) {
                    q.resolve(response.data);
                }, function () {
                    console.log('error');
                });
            return q.promise;
        }

        //Get Reference List
        this.referenceDefer = function () {
            var q = $q.defer();
            ReferenceFactory.list().then(function (response) {
                q.resolve(response.data);
            });
            return q.promise;
        }


    }])

shopApp.service('ImageManager', ['$rootScope', '$q', 'imageFactory',
    function ($rootScope, $q, imageFactory) {


    }])