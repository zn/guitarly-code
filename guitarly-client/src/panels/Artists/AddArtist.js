import React, {Fragment, useState, useEffect} from 'react';
import PropTypes from 'prop-types';
import { useRouter, useLocation, useFirstPageCheck } from '@happysanta/router';
import { IOS, platform } from '@vkontakte/vkui';
import { url_slug } from 'cyrillic-slug'
import { Panel, PanelHeader, PanelHeaderButton, Textarea, FormItem, Button, FormLayout, CustomSelect, Input } from '@vkontakte/vkui';
import { BASE_URL } from '../../config';
import { Icon28ChevronBack, Icon24Back, Icon24Camera } from '@vkontakte/icons';
import {getCookie} from '../../utils';
import { PAGE_ARTIST, PAGE_MAIN } from '../../routers';

const AddArtist = ({id}) => {
	
	const osName = platform();
	const router = useRouter();
	const location = useLocation();
	const isFirstPage = useFirstPageCheck();
	const authToken = getCookie('auth_token', osName);

	const [artistTitle, setTitle] = useState('');
	const [slug, setSlug] = useState('');
	const [photo, setPhoto] = useState();
	const [alternativeNames, setAlternativeNames] = useState('');
	const [artistsList, setArtistsList] = useState(null);

	function save(){
		console.log(photo);
		if(!artistTitle || !photo){
			return;
		}

		// how to send a file → https://stackoverflow.com/a/57595805
		const data = new FormData();
		data.append('title', artistTitle);
		data.append('slug', slug);
		data.append('alternativeNames', alternativeNames);
		data.append('image', photo);

		const requestOptions = {
			method: 'POST',
			headers: {
				'Authorization': `Bearer ${authToken}`},
			body: data
		};

		fetch(BASE_URL + '/artists/add',  requestOptions)
			.then(response => response.json())
			.then(data => {
				router.replacePage(PAGE_ARTIST, { artistId: data });
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
			Добавление артиста
		</PanelHeader>
		
		<FormLayout>
			<FormItem
				top="Название" 
				status={artistTitle ? 'valid' : 'error'}
				bottom='Название артиста'
				>
				<Input name="artistTitle" value={artistTitle} onChange={e=>{
					setTitle(e.target.value);
					setSlug(url_slug(e.target.value));
				}}/>
			</FormItem>
			<FormItem
				top="Slug" 
				status={slug ? 'valid' : 'error'}
				>
				<Input name="slug" value={slug} onChange={e=>setSlug(e.target.value)}/>
			</FormItem>
			
			<FormItem top="Фотография" status={photo ? 'valid' : 'error'} bottom={
				<div>
					Максимальный размер изображения: 5 Мб. <br/>
					Не забудь сжать картинку на https://tinyjpg.com
				</div>
			}>
				{/* <File before={<Icon24Camera />} name="photo" controlSize="m" onChange={e=>setPhoto(e.target.value)}>
					Открыть галерею
				</File> */}
				<Input type='file' before={<Icon24Camera />} accept="image/png, image/gif, image/jpeg"  onChange={e=>setPhoto(e.target.files[0])}/>
			</FormItem>

			<FormItem top="Другие возможные названия" bottom={
				<div>
					Например, группа Кино так же часто ищется запросом "Виктор Цой". <br/> Вводить по одному названию на каждой строке.
				</div>
			}>
				<Textarea name="alternativeNames" value={alternativeNames} onChange={e=>setAlternativeNames(e.target.value)}/>
			</FormItem>

			<FormItem>
				<Button size="l" mode='commerce' stretched onClick={save}>Сохранить</Button>
			</FormItem>
		</FormLayout>
            
	</Panel>
	);}

export default AddArtist;
