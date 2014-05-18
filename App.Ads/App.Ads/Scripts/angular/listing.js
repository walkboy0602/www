
function ListingCreateCtrl($scope, ListingFactory, $filter, $q, ListingManager) {
    $scope.form = {};
    $scope.files = [];
    $scope.cover = null;
    $scope.reference = {};

    var reference;

    $scope.setCategory = function (id, value) {

        $scope.reference.categoryType = [];

        $scope.selected_type = value;

        angular.forEach($filter('filter')(reference, { ParentID: id.toString() }, true), function (val, key) {
            angular.forEach($scope.reference.tags, function (v, k) {
                if (v.ParentID === val.KeyString) {
                    $scope.reference.categoryType.push(v);
                }
            })

        })
        console.log($scope.reference.categoryType);
        $('a[href="#detail"]').tab('show');

        $scope.form.SaleType = id;
    }

    $scope.getClass = function (id) {
        if (id === $scope.form.SaleType) {
            return "category active";
        } else {
            return "category";
        }
    }


    $('#imageuploader').attr('src', '/listing/uploader?id=' + $scope.id);

    $q.all([ListingManager.referenceDefer()]).then(function (results) {
        //var form = results[0];
        reference = results[0];
        $scope.reference.saleType = $filter('filter')(reference, { Type: $scope.Enum.ReferenceType.SaleType.toString() }, true);
        $scope.reference.tags = $filter('filter')(reference, { Type: $scope.Enum.ReferenceType.Tag.toString() }, true);

        //$scope.form.id = form.id;
        //$scope.form.CreateDate = $filter('date')(form.CreateDate, 'dd/MM/yyyy');
        //$scope.form.Title = form.Title;
        //$scope.form.Description = form.Description;
        //$scope.form.Price = form.Price;
        //$scope.form.isWarrently = form.WarrentyDesc === null ? false : true;
        //$scope.form.WarrentyDesc = form.WarrentyDesc;
        //$scope.form.COD = form.COD;
        //$scope.form.OnPay = form.OnPay;
        //$scope.form.PaymentDescription = form.PaymentDescription;

    });

    $scope.presubmit = function () {
        $('#CategoryID').val($scope.form.CategoryID);
    }

    $scope.save = function () {
        ListingFactory.edit($scope.form)
            .success(function (data, status) {
                //$('a[href="#successtab"]').tab('show');
                console.log('success');
            })
            .error(function (data, status) {
                if (status === 400) {
                    $scope.warning(data.Message);
                } else {
                    $scope.error();
                }
            });
    }

}

function ListingImageCtrl($scope, $q, ListingFactory, $filter, uploadManager) {

    $scope.files = [];
    $scope.draftFiles = [];

    $scope.form = {};
    $scope.file = {};

    $scope.form.id = $scope.id;
    $scope.form.CreateDate = $('#CreateDate').val();

    //Retrieve Imge
    $q.all([ListingFactory.getImage($scope.id)]).then(function (results) {
        var data = results[0].data;
        angular.forEach(data, function (v, k) {
            $scope.files.push({
                id: v.id,
                FileName: v.FileName,
                Description: v.Description,
                Src: v.Src,
                Url: "http://assets.monsteritem.com/" + v.Src.replace("####size####", "s1"),
                ListingID: v.ListingID
            })

        });
    });

    //Edit Image
    $scope.edit = function (file) {

        $scope.file = file;

        if (id == undefined) {
            return;
        }

        $('#editModal').modal('show');
    }

    //Edit POST
    $scope.editImage = function () {
        ListingFactory.editImage($scope.file)
            .success(function (data, status) {
                $('#editModal').modal('hide');
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

        if (id == undefined) {
            return;
        }

        $('#deleteModal').modal('show');
    }

    //Delete POST
    $scope.deleteImage = function () {
        ListingFactory.deleteImage($scope.file)
            .success(function (data, status) {
                $('#deleteModal').modal('hide');
                angular.forEach($scope.files, function (v, k) {
                    if (v.id === $scope.file.id) {
                        $scope.files.splice(k, 1);
                    }
                });
            })
            .error(function (data, status) {
                //TODO: redirect
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
            $scope.spinLoading.start($scope.draftFiles.length);
            uploadManager.upload();
        }
    });

    //Do something when image is successfully upload
    $scope.$on('uploadReturn', function (e, response) {
        $scope.spinLoading.end();
        if (response.result !== undefined) {
            console.log(response.result);
            var v = response.result;
            $scope.files.push({
                id: v.id,
                FileName: v.FileName,
                Description: v.Description,
                Src: v.Src,
                Url: "http://assets.monsteritem.com/" + v.Src.replace("####size####", "s1"),
                ListingID: v.ListingID
            });

            //remove draft file
            $scope.draftFiles.splice(0, 1);

            $scope.$apply();
        } else {
            var resp = JSON.parse(response);
            if (resp.result == "Redirect") {
                window.location = resp.url;
            }
        }

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

