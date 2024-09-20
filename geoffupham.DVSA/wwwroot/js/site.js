function AppViewModel(initialVideos) {
    var self = this;
    self.videos = ko.observableArray(initialVideos);

    self.formatSize = function (bytes) {
        if (bytes == 0) return '0 Bytes';
        var k = 1024,
            sizes = ['Bytes', 'KB', 'MB', 'GB'],
            i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    };

    self.playVideo = function (video) {
        console.log('Playing video:', video.name);
        var player = document.getElementById('player');
        player.src = '/DVSAmedia/' + video.path;
        player.play();
    };

    self.uploadVideo = function () {
        console.log('uploadVideo function called');
        var fileInput = document.getElementById('fileInput');
        console.log('Files selected:', fileInput.files.length);
        var formData = new FormData();

        for (var i = 0; i < fileInput.files.length; i++) {
            console.log('Appending file:', fileInput.files[i].name);
            formData.append('files', fileInput.files[i]);
        }

        console.log('Sending fetch request to /api/video/upload');
        fetch('/api/VideoApi/upload', {
            method: 'POST',
            body: formData
        })
            .then(response => {
                console.log('Received response:', response);
                if (!response.ok) {
                    return response.text().then(text => { throw new Error(text) });
                }
                return response.json();
            })
            .then(data => {
                console.log('Upload successful, received data:', data);
                self.videos(data);
                document.getElementById('catalogue-tab').click();
                fileInput.value = '';
                document.getElementById('selectedFiles').textContent = '';
            })
            .catch(error => {
                console.error('An error occurred during upload:', error);
                alert('An error occurred during upload: ' + error.message);
            });
    };
}

function initializeApp() {
    console.log('DOM content loaded');

    document.getElementById('fileInput').addEventListener('change', function (e) {
        var fileNames = Array.from(e.target.files).map(f => f.name).join(', ');
        document.getElementById('selectedFiles').textContent = fileNames;
        console.log('Selected files:', fileNames);
    });

    var initialVideos = window.initialVideos || [];
    console.log('Initializing AppViewModel with videos:', initialVideos);
    var viewModel = new AppViewModel(initialVideos);
    ko.applyBindings(viewModel);

    document.getElementById('uploadForm').addEventListener('submit', function (event) {
        event.preventDefault();
        console.log('Form submitted');
        viewModel.uploadVideo();
    });
}

// Check if the DOM is already loaded
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeApp);
} else {
    initializeApp();
}