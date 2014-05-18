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