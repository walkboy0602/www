
function EnquiryCtrl($scope, EnquiryFactory) {

    $('form').submit(function () {
        if ($(this).valid()) {

            $.ajax({
                url: '/api/enquiry',
                type: 'POST',
                data: $(this).serialize(),
                success: function (result) {
                    $scope.success(result);
                    $scope.$apply();
                },
                error: function (response) {
                    console.log(response);
                }
            });

        }
        return false;
    });
}