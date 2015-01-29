﻿adsApp.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter);
                });
                event.preventDefault();
            }
        });
    };
});

//shopApp.directive('owlCarousel', function () {
//    return {
//        restrict: 'CA',
//        link: function (scope, element, attrs) {
//            scope.data = [];
//            attrs.$observe("data", function (value) {
//                scope.data = JSON.parse(value);
//            });
//        }
//    }
//});

adsApp.directive('fileupload', ['uploadManager', function factory(uploadManager) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $(element).fileupload({
                dataType: 'json',
                autoUpload: false,
                acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
                maxFileSize: 5000000,
                sequentialUploads: true,
                add: function (e, data) {
                    var uploadErrors = [];
                    var acceptFileTypes = /^image\/(gif|jpe?g|png)$/i;
                    if (data.originalFiles[0]['type'].length && !acceptFileTypes.test(data.originalFiles[0]['type'])) {
                        uploadErrors.push('Not an accepted file type');
                    }
                    if (data.originalFiles[0]['size'].length || data.originalFiles[0]['size'] > 5000000) {
                        uploadErrors.push('Filesize is too big, maximum file size allow is 5mb.');
                    }
                    if (uploadErrors.length > 0) {
                        alert(uploadErrors.join("\n"));
                    } else {
                        uploadManager.add(data);
                    }
                },
                submit: function (e, data) {
                    data.formData = scope.form;
                },
                progressall: function (e, data) {
                    uploadManager.setProgress(data);
                },
                done: function (e, data) {
                    uploadManager.done(data);
                },
                fail: function (e, data) {
                    uploadManager.done(data.jqXHR.responseText);
                }
            });

        }
    };
}]);

adsApp.directive('chosen', [function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs, controller) {

            var list = attrs['chosen'];

            scope.$watch(list + '', function () {
                element.trigger('chosen:updated');
            });

            element.selectpicker();

        }

    };
}]);

adsApp.directive('ckEditor', [function () {
    return {
        require: '?ngModel',
        restrict: 'CA',
        link: function (scope, elm, attr, ngModel) {
            var CKEDITOR_BASEPATH = '/scripts/ckeditor/';

            var ck = CKEDITOR.replace(elm[0], {
                toolbarGroups: [
		            { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		            { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
		            { name: 'styles' },
		            { name: 'colors' },
		            '/',
		            { name: 'clipboard', groups: ['clipboard', 'undo'] },
		            { name: 'editing', groups: ['find', 'selection', 'spellchecker'] },
		            { name: 'links' },
		            { name: 'insert' },
		            { name: 'forms' },
		            { name: 'tools' },
		            { name: 'document', groups: ['mode', 'document', 'doctools'] },
		            { name: 'others' }  
                ]
            });

            if (!ngModel) return;

            //sometimes $render would call setData before the ckeditor was ready
            ck.on('instanceReady', function () {
                ck.setData(ngModel.$viewValue);
            });

            ck.on('dialogHide', function () {
                //pasteState is not enough to handle because data from upload image is assigned after that
                scope.$apply(function () {
                    ngModel.$setViewValue(ck.getData());
                });
            });

            ck.on('pasteState', function () {
                scope.$apply(function () {
                    ngModel.$setViewValue(ck.getData());
                });
            });

            ngModel.$render = function (value) {
                ck.setData(ngModel.$viewValue);
            };
        }
    };
}]);

adsApp.directive('ckEditorMini', [function () {
    return {
        require: '?ngModel',
        restrict: 'CA',
        link: function (scope, elm, attr, ngModel) {
            var CKEDITOR_BASEPATH = '/scripts/ckeditor/';
            var ck = CKEDITOR.replace(elm[0], {
                //toolbar: [
                //    { name: 'paragraph', items: ['BulletedList', 'Outdent', 'Indent'] }
                //],
                toolbarGroups: [
		            { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		            { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
		            { name: 'styles' },
		            { name: 'colors' },
		            '/',
		            { name: 'clipboard', groups: ['clipboard', 'undo'] },
		            { name: 'editing', groups: ['find', 'selection', 'spellchecker'] },
		            { name: 'links' },
		            //{ name: 'insert' },
		            //{ name: 'forms' },
		            { name: 'tools' },
		            //{ name: 'document', groups: ['mode', 'document', 'doctools'] },
		            { name: 'others' }  
                ],
                height: 300
                //,uiColor: '#14B8C4'
            });

            if (!ngModel) return;

            //sometimes $render would call setData before the ckeditor was ready
            //ck.on('instanceReady', function () {
            //    ck.setData(ngModel.$viewValue);
            //});

            ck.on('dialogHide', function () {
                //pasteState is not enough to handle because data from upload image is assigned after that
                scope.$apply(function () {
                    ngModel.$setViewValue(ck.getData());
                });
            });

            ck.on('pasteState', function () {
                scope.$apply(function () {
                    ngModel.$setViewValue(ck.getData());
                });
            });

            ngModel.$render = function (value) {
                ck.setData(ngModel.$viewValue);
            };
        }
    };
}]);

adsApp.directive('spinnerClick', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var spinnerButton = angular.element("<button class='btn disabled'><i class='icon-refresh icon-spin'></i> Doing...</button>").hide();
            element.after(spinnerButton);

            element.click(function () {
                spinnerButton.show();
                element.hide();

                scope.spinnerClick(function () {
                    spinnerButton.hide();
                    element.show();
                });
            });
        }
    };
});