// AstroToolkit site-wide JavaScript functions

// Wait for the document to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    var tooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    if (tooltips.length > 0) {
        tooltips.forEach(function(tooltip) {
            new bootstrap.Tooltip(tooltip);
        });
    }

    // Initialize popovers
    var popovers = document.querySelectorAll('[data-bs-toggle="popover"]');
    if (popovers.length > 0) {
        popovers.forEach(function(popover) {
            new bootstrap.Popover(popover);
        });
    }

    // Handle dark mode toggle if it exists
    var darkModeToggle = document.getElementById('dark-mode-toggle');
    if (darkModeToggle) {
        // Check if user has a preference stored
        const savedDarkMode = localStorage.getItem('darkMode');
        if (savedDarkMode === 'true') {
            document.body.classList.add('dark-mode');
            darkModeToggle.checked = true;
        }

        // Toggle dark mode when the switch is clicked
        darkModeToggle.addEventListener('change', function() {
            if (this.checked) {
                document.body.classList.add('dark-mode');
                localStorage.setItem('darkMode', 'true');
            } else {
                document.body.classList.remove('dark-mode');
                localStorage.setItem('darkMode', 'false');
            }
        });
    }
});

// Helper function to format dates
function formatDate(date) {
    const options = { 
        weekday: 'short', 
        year: 'numeric', 
        month: 'short', 
        day: 'numeric' 
    };
    return date.toLocaleDateString(undefined, options);
}

// Helper function to format times
function formatTime(date) {
    const options = { 
        hour: '2-digit', 
        minute: '2-digit'
    };
    return date.toLocaleTimeString(undefined, options);
}

// Helper function to show toast notifications
function showToast(message, type = 'info') {
    // Create toast container if it doesn't exist
    let toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.className = 'toast-container';
        document.body.appendChild(toastContainer);
    }

    // Create toast element
    const toast = document.createElement('div');
    toast.className = 'toast';
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');

    // Set background color based on type
    let bgClass = 'bg-info';
    if (type === 'success') bgClass = 'bg-success';
    if (type === 'warning') bgClass = 'bg-warning';
    if (type === 'error') bgClass = 'bg-danger';

    // Create toast content
    toast.innerHTML = `
        <div class="toast-header ${bgClass} text-white">
            <strong class="me-auto">AstroToolkit</strong>
            <small>Just now</small>
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
        <div class="toast-body">
            ${message}
        </div>
    `;

    // Add to container
    toastContainer.appendChild(toast);

    // Initialize and show toast
    const bsToast = new bootstrap.Toast(toast, {
        autohide: true,
        delay: 5000
    });
    bsToast.show();

    // Remove from DOM after hiding
    toast.addEventListener('hidden.bs.toast', function() {
        toast.remove();
    });
}

// Helper function to handle API errors
function handleApiError(error, fallbackMessage = 'An error occurred while fetching data.') {
    console.error('API Error:', error);
    showToast(fallbackMessage, 'error');

    // Return a standardized error object that can be used by the calling code
    return {
        error: true,
        message: error.message || fallbackMessage,
        status: error.status || 500
    };
}

// Helper function to detect device type
function getDeviceType() {
    const width = window.innerWidth;
    if (width < 576) return 'mobile';
    if (width < 992) return 'tablet';
    return 'desktop';
}

// Function to get user's location with a Promise interface
function getUserLocation() {
    return new Promise((resolve, reject) => {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                position => {
                    resolve({
                        latitude: position.coords.latitude,
                        longitude: position.coords.longitude,
                        accuracy: position.coords.accuracy
                    });
                },
                error => {
                    reject({
                        code: error.code,
                        message: getGeolocationErrorMessage(error.code)
                    });
                }
            );
        } else {
            reject({
                code: 0,
                message: 'Geolocation is not supported by this browser.'
            });
        }
    });
}

// Helper to get friendly error messages for geolocation errors
function getGeolocationErrorMessage(errorCode) {
    switch(errorCode) {
        case 1:
            return 'Location access denied. Please check your privacy settings.';
        case 2:
            return 'Location information is unavailable at this time.';
        case 3:
            return 'Location request timed out. Please try again.';
        default:
            return 'An unknown error occurred while trying to access your location.';
    }
}

// Function to validate and handle form input
function validateFormInput(input, validationRules = {}) {
    let isValid = true;
    let errorMessage = '';

    const value = input.value.trim();

    // Check if required
    if (validationRules.required && value === '') {
        isValid = false;
        errorMessage = 'This field is required.';
    }

    // Check min length
    if (isValid && validationRules.minLength && value.length < validationRules.minLength) {
        isValid = false;
        errorMessage = `Must be at least ${validationRules.minLength} characters.`;
    }

    // Check max length
    if (isValid && validationRules.maxLength && value.length > validationRules.maxLength) {
        isValid = false;
        errorMessage = `Must be no more than ${validationRules.maxLength} characters.`;
    }

    // Check pattern
    if (isValid && validationRules.pattern && !validationRules.pattern.test(value)) {
        isValid = false;
        errorMessage = validationRules.patternMessage || 'Invalid format.';
    }

    // Check custom validation
    if (isValid && validationRules.custom && typeof validationRules.custom === 'function') {
        const customResult = validationRules.custom(value);
        if (customResult !== true) {
            isValid = false;
            errorMessage = customResult;
        }
    }

    // Update UI based on validation result
    const feedbackElement = input.nextElementSibling;
    if (feedbackElement && feedbackElement.classList.contains('invalid-feedback')) {
        feedbackElement.textContent = errorMessage;
    }

    if (isValid) {
        input.classList.remove('is-invalid');
        input.classList.add('is-valid');
    } else {
        input.classList.remove('is-valid');
        input.classList.add('is-invalid');
    }

    return isValid;
}

// Function to animate number counting
function animateCounter(element, targetValue, duration = 1000, prefix = '', suffix = '') {
    const startValue = 0;
    const startTime = performance.now();

    function updateCounter(currentTime) {
        const elapsedTime = currentTime - startTime;

        if (elapsedTime > duration) {
            element.textContent = prefix + targetValue + suffix;
            return;
        }

        const progress = elapsedTime / duration;
        const currentValue = Math.floor(startValue + progress * (targetValue - startValue));
        element.textContent = prefix + currentValue + suffix;

        requestAnimationFrame(updateCounter);
    }

    requestAnimationFrame(updateCounter);
}

// Map phase type to actual image names
const phaseImageMap = {
    0: 'new-moon',
    1: 'waxing-crescent',
    2: 'first-quarter',
    3: 'waxing-gibbous',
    4: 'full-moon',
    5: 'waning-gibbous',
    6: 'last-quarter',
    7: 'waning-crescent'
};

// Get the image name or use placeholder if not available
const availableImages = ['new-moon', 'full-moon', 'waxing-crescent', 'waxing-gibbous', 'placeholder'];
const imageName = phaseImageMap[data.phase.phaseType] || 'placeholder';
const imageToUse = availableImages.includes(imageName) ? imageName : 'placeholder';

$('#moon-phase-image').attr('src', `/images/moon/${imageToUse}.svg`);