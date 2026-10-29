window.registerApplicantFiltersEnterKey = () => {
    window.unregisterApplicantFiltersEnterKey();

    window._applicantFiltersEnterKeyHandler = (e) => {
        if (e.key === 'Enter') {
            const el = document.getElementById('applicant-search');
            if (el) el.click();
        }
    };

    document.addEventListener('keydown', window._applicantFiltersEnterKeyHandler, true);
};

window.unregisterApplicantFiltersEnterKey = () => {
    if (window._applicantFiltersEnterKeyHandler) {
        document.removeEventListener('keydown', window._applicantFiltersEnterKeyHandler, true);
        delete window._applicantFiltersEnterKeyHandler;
    }
};
