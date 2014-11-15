
function EnquiryCtrl($scope, EnquiryFactory, cfpLoadingBar) {


    $('#enquiryForm').submit(function () {
        if ($(this).valid()) {
            $scope.isSending = true;
            cfpLoadingBar.start();
            $scope.$apply();
            $.ajax({
                url: '/enquiry',
                type: 'POST',
                data: $(this).serialize(),
                success: function (result) {
                    Recaptcha.reload();
                    cfpLoadingBar.complete();
                    $scope.isSending = false;
                    if (result.IsSuccess) {
                        $scope.success(result.Content, false);
                    } else {
                        $scope.warning(result.Content, false);
                    }
                    $scope.$apply();
                },
                error: function (response) {
                    Recaptcha.reload();
                    cfpLoadingBar.complete();
                    $scope.error(undefined, false);
                    $scope.isSending = false;
                    $scope.$apply();
                }
            });

        }
        return false;
    });
}