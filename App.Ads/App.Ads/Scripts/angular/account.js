function VerificationCtrl($scope, AccountFactory, $filter, $q) {

    $scope.sendEmail = function () {
        $('#verification-email').modal('show');
        AccountFactory.emailVerification()
        .success(function (data, status) {
            console.log(data);
        })
        .error(function (data, status) {
            console.log(data);
        });
    }
}

function ChangePasswordCtrl($scope, AccountFactory) {
    console.log('ddd');
    $('form').submit(function () {

        console.log('bbb');
        if ($(this).valid()) {

            cfpLoadingBar.start();
            $scope.$apply();
          
            $.ajax({
                url: '/account/edit/changePassword',
                type: 'Post',
                data: $(this).serialize(),
                success: function (result)
                {
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