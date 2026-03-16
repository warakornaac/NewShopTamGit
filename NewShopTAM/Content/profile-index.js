(function ($) {

    function ProfileIndex() {
        var $this = this;

        function intialize() {
            $("#profileFile").change(function () {
                readURL(this);
            });
        }

        function readURL(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    $('#uploading').attr('src', e.target.result);
                }
                reader.readAsDataURL(input.files[0]);
            }
        }

        $this.init = function () {
            intialize();
        }
    }

    $(function () {
        var self = new ProfileIndex();
        self.init();
    })

})(jQuery)