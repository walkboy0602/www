
function RejectCtrl($scope, cfpLoadingBar) {

    $('#rejectForm').submit(function () {

        if ($(this).valid()) {
            cfpLoadingBar.start();
            $scope.$apply();

            $.ajax({
                url: "/admin/listing/reject",
                type: 'POST',
                data: $(this).serialize(),
                success: function (result) {
                    cfpLoadingBar.complete();
                    $scope.success(result.Content);
                },
                error: function (response) {
                    cfpLoadingBar.complete();
                    $scope.error();
                }
            });
        }
        $scope.$apply();
        return false;
    });
}