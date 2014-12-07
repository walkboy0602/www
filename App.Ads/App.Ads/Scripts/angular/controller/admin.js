
function ApproveCtrl($scope, cfpLoadingBar) {
    
    $('#approveForm').submit(function () {
        if ($(this).valid()) {
            $('#approveModal').modal('hide');
            cfpLoadingBar.start();
            $scope.$apply();

            $.ajax({
                url: "/admin/listing/approve",
                type: "POST",
                data: $(this).serialize(),
                success: function (result) {
                    cfpLoadingBar.complete();
                    if (result.StatusCode == 200)
                    {
                        $scope.success(result.Message);
                    } else {
                        $scope.info(result.Message);
                    }
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

function RejectCtrl($scope, cfpLoadingBar) {

    $('#rejectForm').submit(function () {

        if ($(this).valid()) {
            $('#rejectModal').modal('hide');
            cfpLoadingBar.start();
            $scope.$apply();

            $.ajax({
                url: "/admin/listing/reject",
                type: "POST",
                data: $(this).serialize(),
                success: function (result) {
                    cfpLoadingBar.complete();
                    if (result.StatusCode == 200) {
                        $scope.success(result.Message);
                    } else {
                        $scope.info(result.Message);
                    }
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