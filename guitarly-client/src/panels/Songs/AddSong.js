import React, {Fragment, useState, useEffect} from 'react';
import PropTypes from 'prop-types';
import { useRouter, useLocation, useFirstPageCheck } from '@happysanta/router';
import { IOS, platform } from '@vkontakte/vkui';

import { Panel, PanelHeader, PanelHeaderButton, Textarea, FormItem, Button, FormLayout, CustomSelect, Input } from '@vkontakte/vkui';
import { BASE_URL } from '../../config';
import { Icon28ChevronBack, Icon24Back } from '@vkontakte/icons';
import {getCookie} from '../../utils';
import { PAGE_SONG, PAGE_MAIN } from '../../routers';

const AddSong = ({id}) => {
	
	const osName = platform();
	const router = useRouter();
	const location = useLocation();
	const isFirstPage = useFirstPageCheck();
	const authToken = getCookie('auth_token', osName);

	const [songTitle, setTitle] = useState('');
	const [fullTitle, setFullTitle] = useState('');
	const [text, setText] = useState('');
	const [artistId, setArtistId] = useState(null);
	const [artistTitle, setArtistTitle] = useState(null);
	const [artistsList, setArtistsList] = useState(null);

	useEffect(()=>{
		const requestOptions = {
			headers: { 'Authorization': `Bearer ${authToken}`},
		};

		fetch(BASE_URL + '/songs/add',  requestOptions)
			.then(response => response.json())
			.then(data => {
				setArtistsList(data.artistsList.map((artist) => ({
					label: artist.title,
					value: artist.id,
					avatar: artist.picture30,
					description: artist.title,
				  })));
			});
	}, [])

	function save(){
		if(!(artistId && songTitle && fullTitle && text)){
			return;
		}

		// how to send a file → https://stackoverflow.com/a/57595805
		const data = new FormData();
		data.append('title', songTitle);
		data.append('fullTitle', fullTitle);
		data.append('text', text);
		data.append('artistId', artistId);

		const requestOptions = {
			method: 'POST',
			headers: {
				'Authorization': `Bearer ${authToken}`},
			body: data
		};

		fetch(BASE_URL + '/songs/add',  requestOptions)
			.then(response => response.json())
			.then(data => {
				router.replacePage(PAGE_SONG, { songId: data });
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
			Создание песни
		</PanelHeader>
		
		<FormLayout>
			{artistsList &&
			<FormItem top="Артист" status={artistId ? 'valid' : 'error'}>
				<CustomSelect
					id='artistsSelect'
					placeholder="Введите артиста"
					searchable
					options={artistsList}
					onChange={e=>{
						setArtistId(e.target.value);
						setArtistTitle(artistsList[e.target.options.selectedIndex].label);
					}}
				/>
			</FormItem>
			}
			<FormItem
				top="Название" 
				status={songTitle ? 'valid' : 'error'}
				bottom='Название песни (без исполнителя)'
				>
				<Input name="title" value={songTitle} onChange={e=>setTitle(e.target.value)}/>
			</FormItem>

			<FormItem
				top="Полное название" 
				status={fullTitle ? 'valid' : 'error'}
				bottom='Название песни (вместе с исполнителем)'
				>
				<Input name="fullTitle" value={fullTitle} onChange={e=>setFullTitle(e.target.value)}/>
				
				{
					artistTitle && songTitle &&
					<Button style={{marginTop:'10px'}} mode='tertiary' onClick={e=>setFullTitle(e.target.textContent)}>
						{ artistTitle + ' - ' + songTitle }
					</Button>
				}
			</FormItem>

			<FormItem top="Текст песни" status={text ? 'valid' : 'error'}>
				<Textarea name="text" value={text} onChange={e=>setText(e.target.value)}/>
			</FormItem>

			<FormItem>
				<Button size="l" mode='commerce' stretched onClick={save}>Сохранить</Button>
			</FormItem>
		</FormLayout>
            
	</Panel>
	);}

export default AddSong;
