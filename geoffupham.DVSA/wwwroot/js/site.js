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
        var player = document.getElementById('player');
        player.src = '/media/' + video.name; // Adjust the path as needed
        player.play();
    };

    self.uploadVideo = function () {
        var fileInput = document.getElementById('fileInput');
        var formData = new FormData();

        for (var i = 0; i < fileInput.files.length; i++) {
            formData.append('files', fileInput.files[i]);
        }

        fetch('/api/video/upload', {
            method: 'POST',
            body: formData
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => { throw new Error(text) });
                }
                return response.json();
            })
            .then(data => {
                // Refresh the video list
                self.videos(data);
                // Switch to the catalogue tab
                document.getElementById('catalogue-tab').click();
                // Clear the file input
                fileInput.value = '';
                document.getElementById('selectedFiles').textContent = '';
            })
            .catch(error => {
                alert('An error occurred during upload: ' + error.message);
            });
    };
}

// Display selected file names
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('fileInput').addEventListener('change', function (e) {
        var fileNames = Array.from(e.target.files).map(f => f.name).join(', ');
        document.getElementById('selectedFiles').textContent = fileNames;
    });
});