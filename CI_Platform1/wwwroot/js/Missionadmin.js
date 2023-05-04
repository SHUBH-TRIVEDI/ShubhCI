const inputDiv = document.querySelector(".input-div")
const input = document.querySelector("#imageupload")
const output = document.querySelector("#preview")
const imgsender = document.querySelector("#img-select")

let imagesArray = [];
let FilesToUpload = [];
let FilesNameToUpload = [];
let DelImg = [];
$("#preview").html("");

$("#imageupload").on("change", () => {

    const files = input.files
    for (let i = 0; i < files.length; i++) {

        FilesNameToUpload.push(files[i].name)
        var file = files[i]
        var reader = new FileReader();
        reader.onload = function (event) {
            FilesToUpload.push(event.target.result);

        }

        reader.readAsDataURL(file);
        displayImages()
    }


})




function displayImages() {
    $("#preview").html("");

    setTimeout(function () {
        for (let i = 0; i < FilesToUpload.length; i++) {
            $("#preview").append(`<div class="image"><img src="${FilesToUpload[i]}" alt="image">
                                                                     <button type="button" class="position-absolute image-close-btn bg-light" onclick="deleteImage(${i})" style="right:-3px;font-size:15px">&#x2716</button>
                                                           </div>`)
        }

    }, 100)


}

function deleteImage(i) {
    FilesToUpload.splice(i, 1);
    FilesNameToUpload.splice(i, 1);

    displayImages()
}


function MissionAddEdit() {
    var title = $("#title").val();
    var shortdesc = $("#shortdesc").val();
    var description = $("#description").val();
    var country = $("#country").val();
    var city = $("#city").val();
    var orgname = $("#orgname").val();
    var orgdetail = $("#orgdetail").val();
    var misstype = $("#misstype").val();
    var themeid = $("#themeid").val();
    var seats=$("#seats").val();
    var start = $("#start").val();
    var end = $("#end").val();
    var avail = $("#avail").val();
    var skillmiss = $("#skillmiss").val();
    //var video = $("#video").val();
    var MISSIONID = $("#MISSIONID").val();
    var deadline = $("#deadline").val();

    if (title == "" || title == null) {
        $("#title-validation").removeClass("d-none");
    }

    else if (shortdesc == "" || shortdesc == null) {
        $("#shortdesc-validation").removeClass("d-none");
    }
    else if (start == "" || start == null) {
        $("#start-validation").removeClass("d-none");
    }
    else if (end == "" || end == null) {
        $("#end-validation").removeClass("d-none");
    }
    else if (themeid == "" || themeid == null) {
        $("#themeid-validation").removeClass("d-none");
    }
    else if (misstype == "" || misstype == null) {
        $("#misstype-validation").removeClass("d-none");
    }
    else if (orgdetail == "" || orgdetail == null) {
        $("#orgdetail-validation").removeClass("d-none");
    }
    else if (city == "" || city == null) {
        $("#city-validation").removeClass("d-none");
    }
    else if (orgname == "" || orgname == null) {
        $("#orgname-validation").removeClass("d-none");
    }

    else if (country == "" || country == null) {
        $("#country-validation").removeClass("d-none");
    }
    else if (description == "" || description == null) {
        $("#city-validation").removeClass("d-none");
    }

    else if (seats == "" || seats == null) {
        $("#seats-validation").removeClass("d-none");
    }

    else if (deadline == "" || deadline == null) {
        $("#deadline-validation").removeClass("d-none");
    }
    else {

        $.ajax({
            url: '/Admin/Admin/MissionAddEdit',
            type: 'POST',
            data: {
                'images': FilesToUpload, 'seats': seats, 'deadline': deadline,
                'title': title, 'shortdesc': shortdesc, 'description': description, 'orgname': orgname, 'orgdetail': orgdetail, 'misstype': misstype,
                'themeid': themeid, 'country': country, 'city': city, 'end': end, 'start': start, 'avail': avail, 'MISSIONID': MISSIONID, 'skill': skillmiss
            },
            success: function (res) {
                $(".btn-close").click();
                $("#mission").click();
            },
            error: function (res) {
                console.log(res);
                alert("Modal error");
            }
        });
    }
}


function GetMissionData(MISSIONID) {
    $.ajax({
        url: '/Admin/Admin/GetMissionData',
        type: 'GET',
        data: { 'MISSIONID': MISSIONID },
        success: function (res) {
            res = JSON.parse(res);
            console.log(res);
            $("#title").val(res.Title);
            $("#shortdesc").val(res.ShortDescription);
            $("#description").val(res.Description);
            $("#country").val(res.CountryId);
            $("#city").val(res.CityId);
            $("#orgname").val(res.OrganizationName);
            $("#orgdetail").val(res.OrganizationDetail);
            $("#misstype").val(res.MissionType);
            $("#themeid").val(res.ThemeId);
            var startDate = res.StartDate.split('T');
            $("#start").val(startDate[0]);
            var endDate = res.EndDate.split('T');
            $("#end").val(endDate[0]);
            $("#avail").val(res.Availability);
            $("#skill").val(res.Skill);
            //$("#video").val(res.userId);
            $("#MISSIONID").val(res.MissionId);
            var deadl = res.Deadline.split('T');
            $("#deadline").val(deadl[0]);
            $("#seats").val(res.Seatleft);

           // console.log(res.MissionMedia.$values);
            let images = "";
           for (var i = 0; i < (res.MissionMedia.$values).length; i++) {
            //console.log("hii:  " + res.MissionMedia.$values[i].MediaPath);
               images += `<div class="image"><img src="${res.MissionMedia.$values[i].MediaPath}" alt="image">
                                                                     <button type="button" class="position-absolute image-close-btn bg-light" onclick="deleteImage(${i})" style="right:-3px;font-size:15px">&#x2716</button>
                                                           </div>`

            }
            var imageDiv = document.getElementById("preview");
            imageDiv.innerHTML = images;
            //$("#MISSIONID").val(res.MediaPath);
        },
        error: function (res) {
            console.log(res);
            alert("Mission Modal error");
        }
    });

}

function MissionDelete(MISSIONID) {
    $.ajax({
        url: '/Admin/Admin/MissionDelete',
        type: 'GET',
        data: { 'MISSIONID': MISSIONID },
        success: function (res) {

            Swal.fire({
                title: 'Mission get Deleted',
                showClass: {
                    popup: 'animate__animated animate__fadeInDown'
                },
                hideClass: {
                    popup: 'animate__animated animate__fadeOutUp'
                }
            })
            console.log(res);
            $("#mission").click();
        }
    });
}

function nullvalues() {
    mission = document.getElementById("title");
    mission.value = "";
    date = document.getElementById("shortdesc");
    date.value = "";
    hour = document.getElementById("description");
    hour.value = "";
    minute = document.getElementById("country");
    minute.value = "";
    description = document.getElementById("city");
    description.value = "";
    action = document.getElementById("orgname");
    action.value = "";
    mission1 = document.getElementById("orgdetail");
    mission1.value = "";
    date1 = document.getElementById("misstype");
    date1.value = "";
    description1 = document.getElementById("themeid");
    description1.value = "";
    start = document.getElementById("start");
    date1.value = "";
    end = document.getElementById("end");
    date1.value = "";
    start = document.getElementById("start");
    date1.value = "";
    start = document.getElementById("avail");
    date1.value = "";
    start = document.getElementById("skill");
    date1.value = "";
    start = document.getElementById("video");
    date1.value = "";
}