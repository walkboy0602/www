function ListingCtrl($scope, $q) {


    $scope.submit = function () {
        $("#adForm").validationEngine();
        console.log('d');
    }
}

function CategoryCtrl($scope, $q, $filter, $location, CommonFactory, ListingFactory) {
    $scope.groups = [];
    $scope.categoryLv1 = [];
    $scope.categoryLv2 = [];
    $scope.listTypes = [];
    $scope.refenceListType = [];

    $scope.myPromise = $q.all([CommonFactory.getCategory(), CommonFactory.getReference("LT")]).then(function (result) {

        $scope.categories = result[0].data || [];
        $scope.refenceListType = result[1].data || [];

        angular.forEach($scope.categories, function (v, k) {

            if (angular.isUndefinedOrNull(v.ParentID)) {
                $scope.groups.push(v);
            }

        });
    });

    $scope.selectGroup = function (id) {

        $scope.reset();

        angular.forEach($scope.categories, function (v, k) {

            if (v.id === id) {
                $scope.selectedGroup = v;
            }

            if (v.ParentID === id) {
                $scope.categoryLv1.push(v);
            }
        });
    }

    $scope.selectCategoryLv1 = function (id) {
        $scope.categoryLv2.length = 0;
        $scope.options = [];

        angular.forEach($scope.categories, function (v, k) {
            $scope.selectedOption = null;
            if (v.id === id) {
                $scope.selectedLv1Cat = v;
                var arrListType = v.ListType.split(',');

                if (arrListType.length > 0) {
                    $scope.listTypes.length = 0;
                    angular.forEach(arrListType, function (v, k) {
                        $scope.option = {};
                        $scope.option.key = v;
                        $scope.option.value = $filter('filter')($scope.refenceListType, { Code: v }, true)[0].Name;
                        $scope.options.push($scope.option);
                    });

                }
            }

            if (v.ParentID === id) {
                $scope.categoryLv2.push(v);
            }
        });
    }

    $scope.selectCategoryLv2 = function (id) {
        $scope.selectedOption = null;
        angular.forEach($scope.categories, function (v, k) {
            if (v.id === id) {
                $scope.selectedLv2Cat = v;
                $scope.listType = v.ListType.split(',');
            }

        });
    }

    $scope.selectOption = function (key) {
        $scope.selectedOption = key;
    }

    $scope.checkOption = function () {
        return angular.isUndefinedOrNull($scope.selectedOption);
    }

    $scope.reset = function () {
        $scope.categoryLv1.length = 0;
        $scope.categoryLv2.length = 0;
        $scope.options = [];
        $scope.selectedOption = null;
    }

    $scope.submit = function () {
        $scope.form = {};
        $scope.form.CategoryId = angular.isUndefinedOrNull($scope.selectedLv2Cat) ? $scope.selectedLv1Cat.id : $scope.selectedLv2Cat.id;
        $scope.form.ListType = $scope.selectedOption;

        console.log($scope.form);

        $scope.myPromise = $q.all([ListingFactory.create()])
            .then(function (result) {
                data = result[0].data;
                console.log(data);
                //window.location = data.Body.RedirectUrl;
            });
    }

}

angular.isUndefinedOrNull = function (val) {
    return angular.isUndefined(val) || val === null
}

function ListingImageCtrl($scope, $q, ImageFactory, $filter, uploadManager, $route, $routeParams, $window) {

    $scope.files = [];
    $scope.draftFiles = [];

    $scope.form = {};
    $scope.file = {};

    $scope.form.id = $scope.id;

    $scope.id = "12";

    //Retrieve Imge
    $q.all([ImageFactory.get($scope.id)]).then(function (results) {
        var data = results[0].data;
        angular.forEach(data, function (v, k) {
            $scope.files.push({
                id: v.id,
                FileName: v.FileName,
                Description: v.Description,
                Src: v.Src,
                Url: v.UrlDomain + v.Src.replace("####size####", "s0"),
                ListingID: v.ListingID,
                IsCover: v.IsCover,
                CoverCss: v.IsCover ? "cover" : ""
            })
        });
    },
    function (response) {
        $scope.error();
    });


    //Edit Image
    $scope.edit = function (file) {
        $scope.file = file;
        $('#editModal').modal('show');
    }

    //Edit POST
    $scope.editImage = function () {
        $('#editModal').modal('hide');
        ImageFactory.put($scope.file)
            .success(function (data, status) {
                $window.location.reload();
            })
            .error(function (data, status) {
                if (status === 400) {
                    $scope.warning(data.Message);
                } else {
                    $scope.error();
                }
            });
    }

    //Delete Image
    $scope.delete = function (file) {
        $scope.file = file;

        if ($scope.file.id == undefined) {
            return;
        }
        $('#deleteModal').modal('show');
    }

    //Delete POST
    $scope.deleteImage = function () {
        $('#deleteModal').modal('hide');
        ImageFactory.delete($scope.file.id)
            .success(function (data, status) {
                angular.forEach($scope.files, function (v, k) {
                    if (v.id === $scope.file.id) {
                        $scope.files.splice(k, 1);
                    }
                });
            })
            .error(function (data, status) {
                $scope.error();
            });
    }

    //When an image is added
    $scope.$on('fileAdded', function (e, call) {
        var reader = new FileReader();
        reader.readAsDataURL(call);
        //create a new reference object
        reader.onload = function (e) {
            $scope.draftFiles.push({
                Src: e.target.result
            });

            $scope.$apply();
            $scope.totalFiles = $scope.draftFiles.length + $scope.files.length;

            if ($scope.totalFiles > 8) {
                $scope.warning('Cannot upload more than 8 photos.');
                $scope.draftFiles.splice(0, 1);
                $scope.$apply();
            } else {
                $scope.spinLoading.start($scope.draftFiles.length);
                uploadManager.upload();
            }
        }

    });

    //Do something when image is successfully upload
    $scope.$on('uploadReturn', function (e, response) {
        $scope.spinLoading.end();
        if (response.result !== undefined) {
            var v = response.result;
            $scope.files.push({
                id: v.id,
                FileName: v.FileName,
                Description: v.Description,
                Src: v.Src,
                Url: v.UrlDomain + v.Src.replace("####size####", "s0"),
                ListingID: v.ListingID
            });
        } else {
            $scope.warning($scope.errorMessage);
            //if (response.result == "Redirect") {
            //    window.location = resp.url;
            //}
        }
        //remove draft file
        $scope.draftFiles.splice(0, 1);
        $scope.$apply();
    });

}

function ListingSpecCtrl($scope, $q, ListingFactory, $filter) {

    $scope.form = {};
    $scope.form.ListingID = $scope.id;
    $scope.fields = [];

    //Retrieve Data
    $q.all([ListingFactory.getSpecification($scope.id)]).then(

        function (results) {
            var data = results[0].data;
            angular.forEach(data, function (v, k) {
                $scope.fields.push({
                    ListingID: v.ListingID,
                    Attribute: v.Attribute,
                    Text: v.Text,
                    Sort: v.Sort
                });
            });
        },

        //error
        function (response) {
            console.log(response);
        }

    );


    //init validation
    var isValid = false;
    var validate = jQuery("form").validationEngine({
        prettySelect: true,
        onValidationComplete: function (form, status) {
            isValid = status;
        }
    });

    $scope.saveNew = function () {
        validate.validationEngine('validate');

        console.log(isValid);
        if (!isValid) return;

        $scope.fields.push({
            ListingID: $scope.id,
            Attribute: $scope.form.Attribute,
            Text: $scope.form.Text,
            Sort: $scope.fields.length + 1
        });

        $scope.form = {};
    }

    $scope.edit = function (index) {
        console.clear();
        angular.forEach($scope.fields, function (val, key) {

            if (index === key) {
                $scope.fields[key].EditMode = true;
            }

        });
    }

    $scope.cancel = function (index) {
        angular.forEach($scope.fields, function (val, key) {

            if (index === key) {
                $scope.fields[key].EditMode = false;
            }

        });
    }

    $scope.editSave = function (index) {
        angular.forEach($scope.fields, function (val, key) {

            if (index === key) {
                $scope.fields[key].EditMode = false;
            }

        });
    }

    $scope.remove = function (index) {
        angular.forEach($scope.fields, function (val, key) {

            if (index === key) {
                $scope.fields.splice(key, 1);
            }
        });
    }

    $scope.saveData = function () {
        console.log($scope.fields);
        ListingFactory.saveSpecification($scope.fields)
                .success(function (data, status) {
                    console.log('success');
                })
                .error(function (data, status) {
                    console.log('error');
                });
    }

}

function ListingManageCtrl($scope, $q, $filter) {
    $scope.model = {};

    //triggered when modal is about to be shown
    $('#deleteModal').on('show.bs.modal', function (e) {
        //get data-id attribute of the clicked element
        $scope.model.id = $(e.relatedTarget).data('listing-id');
    });
}

function ListingDescriptionCtrl($scope, $q, ListingFactory, $filter) {

    $scope.form = {};
    $scope.form.id = $scope.id;

    //Retrieve Data
    $q.all([ListingFactory.getDescription($scope.id)]).then(

        function (results) {
            var data = results[0].data;
            $scope.form.id = data.id;
            $scope.form.Description = data.Description;
        },

        //error
        function (response) {
            console.log(response);
        }

    );

    $scope.saveData = function () {
        console.log($scope.form);
        ListingFactory.saveDescription($scope.form)
                .success(function (data, status) {
                    console.log('success');
                })
                .error(function (data, status) {
                    console.log('error');
                });
    }

}

function ListingOptionCtrl($scope, $q, ListingFactory, $filter) {

    $scope.form = {};
    $scope.fields = [];

    $scope.add = function () {
        $scope.mode = 'addnew';
        $scope.form = {};
        $scope.form.ListingID = $scope.id;
        $('#formModal').modal('show');
    }

    //Retrieve Data
    $q.all([ListingFactory.getOption($scope.id)]).then(

        function (results) {
            var data = results[0].data;
            angular.forEach(data, function (v, k) {
                $scope.fields.push({
                    ListingID: v.ListingID,
                    Title: v.Title,
                    OriginalPrice: v.OriginalPrice,
                    DiscountedPrice: v.DiscountedPrice,
                    Quantity: v.Quantity,
                    LinkToImage: v.LinkToImage,
                    Sort: v.Sort
                });
            });
        },

        //error
        function (response) {
            console.log(response);
        }

    );

    //init validation
    var isValid = false;
    var validate = jQuery("form").validationEngine({
        prettySelect: true,
        onValidationComplete: function (form, status) {
            isValid = status;
        }
    });

    $scope.saveNew = function () {
        validate.validationEngine('validate');
        console.log(isValid);

        if (!isValid) return;

        ListingFactory.saveOption($scope.form)
                .success(function (data, status) {
                    console.log('success');
                    $scope.fields.push({
                        ListingID: data.ListingID,
                        Title: data.Title,
                        OriginalPrice: data.OriginalPrice,
                        DiscountedPrice: data.DiscountedPrice,
                        Quantity: data.Quantity,
                        LinkToImage: data.LinkToImage,
                        Sort: data.Sort
                    });
                    $('#formModal').modal('hide');
                })
                .error(function (data, status) {
                    console.log('error');
                });
    }

    //OPEN MODEL - EDIT
    $scope.edit = function (index) {
        angular.forEach($scope.fields, function (val, key) {
            if (index === key) {
                $scope.form = $scope.fields[key];
                $scope.mode = 'edit';
                $('#formModal').modal('show');
            }
        });
    }

    $scope.editSave = function () {
        validate.validationEngine('validate');
        console.log(isValid);

        if (!isValid) return;

        ListingFactory.editOption($scope.form)
                .success(function (data, status) {
                    console.log('success');

                    angular.forEach($scope.fields, function (val, key) {
                        if (data.Sort === val.Sort) {
                            val.ListingID = data.ListingID,
                            val.Title = data.Title,
                            val.OriginalPrice = data.OriginalPrice,
                            val.DiscountedPrice = data.DiscountedPrice,
                            val.Quantity = data.Quantity,
                            val.LinkToImage = data.LinkToImage,
                            val.Sort = data.Sort
                        }
                    });

                    $('#formModal').modal('hide');
                })
                .error(function (data, status) {
                    console.log('error');
                });
    }

    //OPEN MODEL - DELETE
    $scope.delete = function (index) {
        angular.forEach($scope.fields, function (val, key) {
            if (index == key) {
                $scope.form = $scope.fields[key];
                $('#deleteModal').modal('show');
            }
        });
    }

    $scope.deleteConfirm = function () {
        console.log('d');
        ListingFactory.deleteOption($scope.form)
             .success(function (data, status) {
                 angular.forEach($scope.fields, function (val, key) {
                     if (val.Sort === data.Sort) {
                         $scope.fields.splice(key, 1);
                     }
                 });
                 $('#deleteModal').modal('hide');
             })
            .error(function (data, status) {
                console.log('error');
            });
    }

}

function RegionCtrl($scope, CommonFactory) {

    $scope.getArea = function () {
        CommonFactory.getArea($('#LocationId').val())
            .success(function (result, status) {
                var data = result.Body;
                var opt = '<option value="">--- All Area ---</option>';
                angular.forEach(data, function (v, k) {
                    opt += '<option value="' + v.Value + '">' + v.Text + '</option>';
                });

                console.log(result);
                $('select[id=AreaId]').html(opt);

                var aid = getParameterByName('aid');
                if (aid != '') {
                    $('select[id="AreaId"] option[value=' + aid + ']').prop('selected', true);
                }
            })
            .error(function (data, status) {
                Console.log('error occured while populating area');
            });
    }

    if ($('#LocationId').val() > 0 && $('#AreaId').children('option').length <= 1) {
        $scope.getArea();
    }

    $('#LocationId').change(function () {
        $scope.getArea();
    });

}

function ListingMapCtrl($scope, $q) {


    function initialize() {

        $scope.map = {};

        if ($('#Lat').val() != undefined) {
            $scope.map.lat = $('#Lat').val();
            $scope.map.lng = $('#Lng').val();
        } else {
            $scope.map.lat = 3.1357;
            $scope.map.lng = 101.6880;
        }


        var map = new google.maps.Map(document.getElementById('map-canvas'), {
            center: new google.maps.LatLng($scope.map.lat, $scope.map.lng),
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            zoom: 16
        });

        var marker = new google.maps.Marker({
            position: new google.maps.LatLng($scope.map.lat, $scope.map.lng),
            map: map,
            title: "Your location is here.",
            draggable: true
        });

        //var defaultBounds = new google.maps.LatLngBounds(
        //    new google.maps.LatLng($scope.map.lat, $scope.map.lng));
        //map.setBounds(defaultBbounds);

        // Create the search box and link it to the UI element.
        var input = /** @type {HTMLInputElement} */(
            document.getElementById('map-searchbox'));
        map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

        var searchBox = new google.maps.places.SearchBox(
          /** @type {HTMLInputElement} */(input));

        // Listen for the event fired when the user selects an item from the
        // pick list. Retrieve the matching places for that item.
        google.maps.event.addListener(searchBox, 'places_changed', function () {
            var places = searchBox.getPlaces();

            if (places.length == 0) {
                return;
            }

            // For each place, get the icon, place name, and location.
            //markers = [];
            var bounds = new google.maps.LatLngBounds();
            for (var i = 0, place; place = places[i]; i++) {

                // Clear previous marker
                marker.setMap(null);

                // Create a marker for each place.
                marker = new google.maps.Marker({
                    map: map,
                    title: place.name,
                    position: place.geometry.location,
                    draggable: true,
                    animation: google.maps.Animation.DROP
                });

                //markers.push(marker);

                bounds.extend(place.geometry.location);
                setLocation(place.geometry.location, place.name);
            }

            map.fitBounds(bounds);
            map.setZoom(16);

            google.maps.event.addListener(map, 'click', function (event) {
                marker.setPosition(event.latLng);
                setLocation(event.latLng);
            });

            google.maps.event.addListener(marker, 'dragend', function (event) {
                setLocation(event.latLng);
            });
        });

        // Bias the SearchBox results towards places that are within the bounds of the
        // current map's viewport.
        google.maps.event.addListener(map, 'bounds_changed', function () {
            var bounds = map.getBounds();
            searchBox.setBounds(bounds);
        });

        google.maps.event.addListener(map, 'click', function (event) {
            marker.setPosition(event.latLng);
            setLocation(event.latLng);
            //$scope.map.lat = marker.getPosition().lat();
            //$scope.map.lng = marker.getPosition().lng();
            //$scope.$apply();
        });

        google.maps.event.addListener(marker, 'dragend', function (event) {
            setLocation(event.latLng);
        });


    }

    //var map = google.maps.event.addDomListener(window, 'click', initialize);
    $scope.mapLoaded = false;
    $scope.loadMap = function () {
        if ($scope.mapLoaded == false) {
            initialize();
        }
        $scope.mapLoaded = true;
    }

    function setLocation(location, place) {
        $scope.map.lat = location.lat();
        $scope.map.lng = location.lng();
        $('#Lat').val($scope.map.lat);
        $('#Lng').val($scope.map.lng);
        if (place !== undefined) {
            $('#Place').val(place);
        }
        $scope.$apply();
    }
}