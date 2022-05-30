import Cookies from 'js-cookie'

import {IOS} from '@vkontakte/vkui'

export function declOfNum(number, words) {  
    return words[(number % 100 > 4 && number % 100 < 20) ? 2 : [2, 0, 1, 1, 1, 2][(number % 10 < 5) ? Math.abs(number) % 10 : 5]];
}

import { BASE_URL } from './config';


export async function auth(user, platform){
    // логиним / регаем пользователя
    const requestOptions = {
        method: 'POST',
        body: new URLSearchParams({
            'queryString': window.location.search,
            'firstName': user.first_name,
            'lastName': user.last_name,
            'sex': user.sex,
            'photo100': user.photo_100,
            'photo200': user.photo200,
            'photoMaxOrig': user.photo_max_orig,
        }),
    };
    await fetch(BASE_URL + '/users/auth', requestOptions)
        .then(response => response.json())
        .then(data => {
            if(platform === IOS)
                localStorage.setItem('auth_token', data.token);
            else
                Cookies.set('auth_token', data.token);
        })
        .catch(e=>{
            console.log('Error while trying to authenticate user. Error message: ' + e)
        });
}

export const getCookie = (name, platform) => {
    if(platform === IOS)
        return localStorage.getItem(name);
    else
        return Cookies.get(name);
}