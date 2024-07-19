$().ready(function () {

    $("#update").click(function (event) {
        event.preventDefault();

        var title = $("#update_title").val();
        var code = $("#update_code").val();
        var courseType = $("#update_courseType").val();
        var units = $("#update_units").val();
        var block = $("#update_block").val();
        var description = $("#update_description").val();

        // Collect the schedule values
        var schedule = [];
        $("input[name='schedule']:checked").each(function () {
            schedule.push($(this).val());
        });

        // Collect times for each day
        var startTimes = [];
        var endTimes = [];
        schedule.forEach(function (day) {
            startTimes.push($("#update-time-" + day + "-start").val());
            endTimes.push($("#update-time-" + day + "-end").val());
        });

        // Combine startTimes and endTimes
        var timePairs = startTimes.map((startTime, index) => `${startTime} - ${endTimes[index]}`);

        // Client-side validation
        if (!title || !code || !courseType || !units || !block || !description || schedule.length === 0 || timePairs.length === 0 || timePairs.includes(" - ")) {
            alert("Please fill in all required fields, including schedule and time.");
            return;
        }

        $.post("../Home/CourseUpdate", {
            title: title,
            code: code,
            courseType: courseType,
            units: units,
            block: block,
            description: description,
            schedule: schedule.join(','),
            time: timePairs.join(',')
        }, function (data) {
            if (data[0].mess == 0) {
                alert("The data was successfully updated");
                location.reload();
            } else {
                alert("Update failed: " + data[0].error);
            }
        });
    });


    $("[id^=edit_]").click(function (event) {
        event.preventDefault();
        var code = $(this).data("course-code");
        $.post("../Home/CourseSearch", { code: code }, function (data) {
            if (data[0].mess == 0) {
                $("#update_title").val(data[0].title);
                $("#update_code").val(data[0].code);
                $("#update_courseType").val(data[0].course_type);
                $("#update_units").val(data[0].units);
                $("#update_block").val(data[0].block);
                $("#update_description").val(data[0].description);

            } else {
                alert("No Subject Found!");
            }
        });
    });



    $("[id^=delete_]").click(function (event) {
        event.preventDefault();
        var code = $(this).data("course-code");

        $.post("../Home/deleteCourse", {
            code: code

        }, function (data) {
            if (data[0].mess == 0) {
                alert('Data was successfully removed');
                location.reload();
            }
        });
    });
});
