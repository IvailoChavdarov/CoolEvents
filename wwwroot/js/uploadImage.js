﻿//connections to the Cloudinary API
const CLOUDINARY_URL = 'https://api.cloudinary.com/v1_1/ddo3vrwcb/image/upload';
const CLOUDINARY_UPLOAD_PRESET = 'k4nawbiw';

//image input
const image = document.querySelector('#eventImageUpload');

//submitting form containing image upload
document.getElementById('eventSubmitButton').addEventListener('click', (e) => {
    e.preventDefault();
    //checks if image is uploaded or connected by url
    if (image.files.length > 0) {
        //uploads file to cloudinary and sets input value to posted image
        const file = image.files[image.files.length - 1];
        const formData = new FormData();
        formData.append('file', file);
        formData.append('upload_preset', CLOUDINARY_UPLOAD_PRESET);
        fetch(CLOUDINARY_URL, {
            method: 'POST',
            body: formData,
        })
            .then(response => response.json())
            .then((data) => {
                if (data.secure_url !== '') {
                    //sets image url upload to uploaded image url
                    const uploadedFileUrl = data.secure_url;
                    document.getElementById('eventImageInput').value = uploadedFileUrl;
                }
            }).then(() => {
                document.getElementById('event-form').submit()
            })
    }
    else {
        document.getElementById('event-form').submit()
    }

})

//validates the uploaded image
image.addEventListener('change', (e) => {
    if (e.target.files[image.files.length - 1].name.endsWith(".png") || e.target.files[image.files.length - 1].name.endsWith(".jpg") || e.target.files[image.files.length - 1].name.endsWith(".jpeg") | e.target.files[image.files.length - 1].name.endsWith(".gif")) {
        if (e.target.files[0].size < 2000000) {
            document.getElementById('eventImageInput').value = e.target.files[image.files.length - 1].name
            document.getElementById('imageValidationText').innerText = ""
        }
        else {
            console.log("Image too large")
            document.getElementById('imageValidationText').innerText = "Image is too large"
            image.value = null;
            document.getElementById('eventImageInput').value = ""
        }
    }
    else {
        document.getElementById('imageValidationText').innerText = "Image should be in png, jpg, jpeg or gif format"
        image.value = null;
        document.getElementById('eventImageInput').value = ""
    }
})