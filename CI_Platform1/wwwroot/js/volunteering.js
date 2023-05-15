function Pdf(url) {
    window.open(url);
}

let slideIndex = 1;
showSlides(slideIndex);

// Next/previous controls
function plusSlides(n) {
    showSlides(slideIndex += n);
}
function LoginAlert() {
    Swal.fire('Please login first.');
}
// Thumbnail image controls
function currentSlide(n) {
    showSlides(slideIndex = n);
}

function showSlides(n) {
    let i;
    let slides = document.getElementsByClassName("mySlides");
    let dots = document.getElementsByClassName("demo");
    let captionText = document.getElementById("caption");
    if (n > slides.length) { slideIndex = 1 }
    if (n < 1) { slideIndex = slides.length }
    for (i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
    }
    for (i = 0; i < dots.length; i++) {
        dots[i].className = dots[i].className.replace(" active", "");
    }
    slides[slideIndex - 1].style.display = "block";
    dots[slideIndex - 1].className += " active";
    // captionText.innerHTML = dots[slideIndex-1].alt;
}



// Tabs js
const tabs = document.querySelectorAll('.tab');
const tabContents = document.querySelectorAll('.tab-content-item');

tabs.forEach(tab => {
    tab.addEventListener('click', function () {
        // remove active class from all tabs and tab content
        tabs.forEach(tab => tab.classList.remove('active'));
        tabContents.forEach(content => content.classList.remove('active'));

        // add active class to current tab and its corresponding tab content
        this.classList.add('active');
        document.getElementById(this.dataset.tabContent).classList.add('active');
    });
});


//--------------------------Add to Favroite Mission--------------------------------------------


function addToFav(missonid) {
    $.ajax({
        type: 'GET',
        url: "/Employee/Home/addToFavourite",
        data: { 'missonid': missonid },
        success: function (res) {
            $('.addFav').html($(res).find('.addFav').html());

        },
        error: function () {
            Swal.fire('Please login first.');
        }

    });
}

function Apply(missonid) {
    $.ajax({
        type: 'GET',
        url: "/Employee/Home/Applied",
        data: { 'missonid': missonid },
        success: function (res) {
            $('.apply').html($(res).find('.apply').html());
        },
        error: function () {
            Swal.fire('please login first.');
        }

    });
}

//Rating
function addRating(starId, missionId, Id) {


    $.ajax({
        url: '/Employee/Home/Addrating',
        type: 'POST',
        data: { missionId: missionId, Id: Id, rating: starId },
        success: function (response) {

            $('#starsr').html($(response).find('#starsr').html());
            $('#avgrating').html($(response).find('#avgrating').html());

        },
        error: function () {
            Swal.fire('please login first.');
        }
    });
}

//Comments---------------------------
function PostComment() {
    var textBox = document.getElementById("commentarea");
    var commentVal = textBox.value;
    const params = new URLSearchParams(window.location.search);
    const query = params.get('missionid');

    if (commentVal == "") {
        $('#cmtErr').removeClass('d-none');
    }
    else {

        $.ajax({
            url: "/Employee/Home/PostComment",
            data: { missionId: query, Content: commentVal },
            success: function (result) {
                $('.commentdiv').html($(result).find('.commentdiv').html());
            },
            error: function () {
                Swal.fire('Error : Comment has not been posted');
            }
        });
    }
}
function Entercmt() {
    $('#cmtErr').addClass('d-none');
}

//coworker
function sendmail(id) {
    const mail = Array.from(document.querySelectorAll('input[name="mail"]:checked')).map(el => el.id);
    var send = document.getElementById("sendmail");
    send.innerHTML = "Sending"
    $.ajax({
        url: '/Employee/Home/Sendmail',
        type: 'POST',
        data: { userid: mail, id: id },
        success: function (result) {
            Swal.fire('Recomendations sent successfully');

            const checkboxes = document.querySelectorAll('input[name="mail"]:checked');
            checkboxes.forEach((checkbox) => {
                checkbox.checked = false;
            });
            send.innerHTML = "sent"
        },
        error: function () {
            // Handle error response from the server, e.g. show an error message to the user
            Swal.fire('Error: Could not recommend mission.');

        }
    });

}
