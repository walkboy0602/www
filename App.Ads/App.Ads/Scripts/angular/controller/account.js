function VerificationCtrl($scope, AccountFactory, $filter, $q) {

    $scope.sendEmail = function () {
        $('#verificationModal').modal('show');
        AccountFactory.emailVerification()
            .success(function (data, status) {
                if (data.StatusCode !== 200) {
                    $scope.error(data.Message);
                } else {
                    $scope.success(data.Message);
                }
            })
            .error(function (data, status) {
                $scope.error(data.Message);
            });
    }
}

function ChangePasswordCtrl($scope, AccountFactory) {

    $('form').submit(function () {
        if ($(this).valid()) {

            cfpLoadingBar.start();
            $scope.$apply();

            $.ajax({
                url: '/account/edit/changePassword',
                type: 'Post',
                data: $(this).serialize(),
                success: function (result) {
                    cfpLoadingBar.complete();
                    if (result.IsSuccess) {
                        $scope.success(result.Content);
                    } else {
                        $scope.warning(result.Content);
                    }
                    $scope.$apply();
                },
                error: function (response) {
                    cfpLoadingBar.complete();
                    $scope.error();
                    $scope.$apply();
                }
            });


        };

        return false;

    });
}