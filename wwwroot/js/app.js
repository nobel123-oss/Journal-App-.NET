// JavaScript utilities for JournalApp
window.blazorHelpers = {
    // Download file utility
    downloadFile: function (filename, contentType, content) {
        const blob = new Blob([content], { type: contentType });
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
    },

    // Get current theme
    getCurrentTheme: function () {
        return document.documentElement.classList.contains('dark-theme') ? 'dark' : 'light';
    },

    // Set theme
    setTheme: function (theme) {
        if (theme === 'dark') {
            document.documentElement.classList.add('dark-theme');
            document.documentElement.classList.remove('light-theme');
        } else {
            document.documentElement.classList.add('light-theme');
            document.documentElement.classList.remove('dark-theme');
        }
    }
};
