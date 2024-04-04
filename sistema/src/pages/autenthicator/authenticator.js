import jQuery from 'jquery';

import { AuthService, UserInfoService } from '../../js/core/index.js'

import '../../js/main.js'

jQuery(async () => {
    try {
        await AuthService.initialize();

        const userName = UserInfoService.getName();

        jQuery("#keyusername").html(userName);

        jQuery("#camp_user").val(JSON.stringify(UserInfoService.getProfile()));
    } catch (err) {
        alert(err.message);
    }
})