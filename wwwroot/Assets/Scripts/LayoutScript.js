function GetResearchByUser(userEmail) {
    window.axios.get('/api/app/userInformation/checkUserCanAccessDefinedTopic?userEmail=' + userEmail)
        .then(function (response) {
            if (response.data.researchExists === true) {
                $("#activeResearch").show();
                $("#definedTopic").hide();
                $("#research").show();
            }
            else if (response.data.applicationExists === true && response.data.isResearchTopicDefinedByDepartment === true) {
                $("#activeResearch").hide();
                $("#definedTopic").show();
                $("#research").show();
            }
            else if (response.data.applicationExists === true &&
                response.data.isResearchTopicDefinedByDepartment !== true &&
                response.data.researchExists !== true) {
                $("#researchProposal").show();
                $("#activeResearch").hide();
                $("#definedTopic").hide();
                $("#research").show();
            }
        });
}
