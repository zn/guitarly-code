import React, {Fragment, useState, useEffect} from 'react';
import PropTypes from 'prop-types';
import { useRouter, useLocation, useFirstPageCheck } from '@happysanta/router';
import { IOS, Placeholder, platform } from '@vkontakte/vkui';

import { Panel, PanelHeader, PanelHeaderButton, Textarea, FormItem, Button, FormLayout, ScreenSpinner, Avatar, Input } from '@vkontakte/vkui';
import { BASE_URL } from '../../config';
import { Icon28ChevronBack, Icon24Back } from '@vkontakte/icons';
import {getCookie} from '../../utils';
import { PAGE_SONG, PAGE_MAIN } from '../../routers';

const EditSong = ({ id }) => {
	
	const osName = platform();
	const router = useRouter();
	const location = useLocation();
	const isFirstPage = useFirstPageCheck();

	var {songId} = location.getParams(); 
	const authToken = getCookie('auth_token', osName);

	const [songModel, setSongModel] = useState(null);

	const [title, setTitle] = useState('');
	const [fullTitle, setFullTitle] = useState('');
	const [text, setText] = useState('');

	useEffect(()=>{
		const requestOptions = {
			headers: { 'Authorization': `Bearer ${authToken}`},
		};

		fetch(BASE_URL + '/songs/edit/' + songId,  requestOptions)
			.then(response => response.json())
			.then(data => {
				setSongModel(data);
				setTitle(data.title);
				setFullTitle(data.fullTitle);
				setText(data.text);
			});
	},[])


	function save(){
		const requestOptions = {
			method: 'PUT',
			headers: {
				'Authorization': `Bearer ${authToken}`},
			body: new URLSearchParams({
				'title': title,
				'fullTitle': fullTitle,
				'text': text
			})
		};

		fetch(BASE_URL + '/songs/edit/' + songId,  requestOptions)
			.then(response => response.json())
			.then(data => {
				console.log(data);
				router.popPage();
			});
	}

	return(
	<Panel id={id}>
		<PanelHeader
				left={<PanelHeaderButton onClick={() => {
					if (isFirstPage) {
						router.replacePage(PAGE_MAIN)
					} else {
						router.popPage()
					}
				}}
					style={{ backgroundColor: 'transparent' }}>
					{osName === IOS ? <Icon28ChevronBack /> : <Icon24Back />}
				</PanelHeaderButton>}
			>
				Редактирование песни
			</PanelHeader>
			
		{!songModel && <ScreenSpinner />}
		{songModel &&
			<FormLayout>
				<FormItem
					top="Название" 
					status={title ? 'valid' : 'error'}
					bottom='Название песни (без исполнителя)'
					>
					<Input name="title" value={title} onChange={e=>setTitle(e.target.value)}/>
				</FormItem>

				<FormItem
					top="Полное название" 
					status={fullTitle ? 'valid' : 'error'}
					bottom='Название песни (вместе с исполнителем)'
					>
					<Input name="fullTitle" value={fullTitle} onChange={e=>setFullTitle(e.target.value)}/>
				</FormItem>

				<FormItem top="Текст песни" status={text ? 'valid' : 'error'}>
					<Textarea name="text" value={text} onChange={e=>setText(e.target.value)}/>
				</FormItem>

				<FormItem>
					<Button size="l" mode='commerce' stretched onClick={save}>Сохранить</Button>
				</FormItem>
			</FormLayout>
		}
            
	</Panel>
	);}

export default EditSong;
