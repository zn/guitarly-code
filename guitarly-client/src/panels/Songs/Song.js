/* global VK */
import React, { useState, useEffect, Fragment } from 'react';
import PropTypes from 'prop-types';

import { Panel, PanelHeader,PopoutWrapper,Button,Spacing,Snackbar, ModalPage,ModalPageHeader,PanelHeaderClose,ModalRoot, PanelHeaderButton, Cell, ScreenSpinner, List, PanelHeaderContext, PanelHeaderContent, Group, Separator, SimpleCell, Header, Div, Avatar, IconButton, HorizontalScroll, HorizontalCell, RichCell } from '@vkontakte/vkui';

import { IOS, usePlatform, useAdaptivity, ViewWidth  } from '@vkontakte/vkui';

import { useFirstPageCheck, useParams, useLocation, useRouter } from '@happysanta/router';
import { PAGE_SONG, PAGE_ARTIST, PAGE_MAIN, PAGE_EDIT_SONG, MODAL_SHARE_MOBILE, MODAL_TRANSPOSE } from '../../routers';

import { Icon28FavoriteOutline, Icon28EditOutline, Icon28DoneOutline,
		 Icon28UnfavoriteOutline, Icon12ChevronOutline, Icon16Done, Icon28SettingsOutline,
		 Icon16Minus,Icon16Add,
	  	 Icon24Back, Icon28ChevronBack, Icon16Dropdown, Icon28DeleteOutline, Icon28ShareOutline } from '@vkontakte/icons';



import { declOfNum, auth, getCookie } from '../../utils';
import './Song.css';
import jwt_decode from "jwt-decode";
import { Parser } from 'html-to-react'

import { BASE_URL, VK_APP_URL } from '../../config';
import bridge from "@vkontakte/vk-bridge";


const Song = ({ id, setModalShareMobile, setModalTranspose }) => {

	const [tone, setTone] = useState(0);
	const osName = usePlatform();
	const { viewWidth } = useAdaptivity();
  	const isMobile = window.innerWidth < 650; //viewWidth <= ViewWidth.MOBILE;
	const router = useRouter();
	const isFirstPage = useFirstPageCheck();
	let authToken = getCookie('auth_token', osName);
	
	const location = useLocation(true);
	const {songId} = location.getParams();
	const [songModel, setSongModel] = useState(null);
	const [songText, setSongText] = useState(null);
	const [contextOpened, setContextOpened] = useState(false);
	const [isFavorite, setIsFavorite] = useState(false);
	const [snackbar, setSnackbar] = useState(null);
	const [isModer, setIsModer] = useState(false);
	
	function getTransposeModal(songModel){
		var currentTone = parseInt(document.getElementById('currentToneValue').innerText);
		return (
			<div style={{textAlign:'center'}}>
				<Button size='l' mode='secondary' onClick={() => {
					var newTone = transpose(-1, songModel.song.id);
					document.getElementById('toneCounterModal').innerText = ['','+'][+(newTone > 0)] + newTone;
					currentTone = newTone;
				}}><Icon16Minus/></Button>
				
				<div style={{padding: '10px', fontSize:'20px', display:'inline-block'}}>
					<span id='toneCounterModal'>{['','+'][+(currentTone > 0)] + currentTone}</span>
				</div>
				<Button size='l' mode='secondary' onClick={() => {
					var newTone = transpose(+1, songModel.song.id);
					document.getElementById('toneCounterModal').innerText = ['','+'][+(newTone > 0)] + newTone;
					currentTone = newTone;
				}}><Icon16Add/></Button>
			</div>
		);
	}

	function getShareModal(songModel){
		return (
			<Fragment>
				<SimpleCell onClick={()=>shareWall(songModel, isMobile)}>На своей стене</SimpleCell>
				<SimpleCell onClick={shareIM}>В личном сообщении</SimpleCell>
			</Fragment>
		)
	}

	function isTokenValid(token){
		var validTo = new Date(token.exp*1000);
		var now = new Date();
		var remainValidHours = (validTo - now) / 1000 / 60 / 60;
		return remainValidHours > 1;
	}

	useEffect(() => {
		window.scrollTo(0, 0);
		setTone(0);

		async function fetchData() {
			if(!authToken){ // handle bag with token
				const user = await bridge.send('VKWebAppGetUserInfo');
				await auth(user, osName);
				authToken = getCookie('auth_token', osName);
			}
			var decodedToken = jwt_decode(authToken);
			if(!isTokenValid(decodedToken)){
				const user = await bridge.send('VKWebAppGetUserInfo');
				await auth(user, osName);
				authToken = getCookie('auth_token', osName);
				decodedToken = jwt_decode(authToken);
			}
			var userRole = decodedToken.role;
			setIsModer(userRole === "moder" || userRole === "admin");
			const requestOptions = {
				headers: { 'Authorization': `Bearer ${authToken}`},
			};

			fetch(BASE_URL + '/songs/' + songId,  requestOptions)
				.then(response => response.json())
				.then(data => {
					if(data.status){
						router.replacePage(PAGE_MAIN);
						return;
					}
					setSongModel(data);
					setSongText(data.song.text);
					setIsFavorite(data.isFavorite);
					// setting modals
					setModalTranspose(getTransposeModal(data));
					setModalShareMobile(getShareModal(data));

					// ↓ uncomment if u want to turn on comments
					
					// var comments = document.getElementById('vk_comments');
					// if(comments && !comments.innerHTML){
					// 	eval(`VK.Widgets.Comments("vk_comments", {page_id: '/song/${songId}'})`); // loading comments
					// }
				})
				.catch(e=>{
					console.log(e);
					router.replacePage(PAGE_MAIN);
				})
		}
		if(songId)
			fetchData();
	}, [songId]);

	function changeFavoriteStatus(){
		const requestOptions = {
			method: 'POST',
			headers: { 'Authorization': `Bearer ${authToken}` },
		};

		fetch(BASE_URL + '/songs/favorite?songId=' + songId, requestOptions)
			.then(response => response.json())
			.then(data => setIsFavorite(data))
			.catch(e=>console.error(e));
	}


	function deleteSong(){
		if(window.confirm('Удалить песню?')){
			const requestOptions = {
				method: 'DELETE',
				headers: { 'Authorization': `Bearer ${authToken}` },
			};
	
			fetch(BASE_URL + '/songs/delete/' + songId, requestOptions)
				.then(response => response.json())
				.then(data => router.popPage());
		}
	}

	function publishSong(){
		if(window.confirm('Опубликовать песню?')){
			const requestOptions = {
				method: 'POST',
				headers: { 'Authorization': `Bearer ${authToken}` },
			};
	
			fetch(BASE_URL + '/songs/publish/' + songId, requestOptions)
				.then(response => response.json())
				.then(data => router.popPage());
		}
	}

	// if 'song' param specified, then it means that we call it from modal
	function shareWall(song = null, mobile = true){
		var songTitle, closeModal;
		if(song){
			songTitle = song.song.fullTitle;
			closeModal = mobile;
		}
		else{
			songTitle = songModel.song.fullTitle;
			closeModal = isMobile;
		}
		bridge.send("VKWebAppShowWallPostBox", {
			"message": `"${songTitle}" на Guitarly`,
			"attachments": VK_APP_URL+'#'+location.route.getLocation().split('?',1)[0]
		  })
		  .then(_=>{
			  setSnackbar(
				<Snackbar
					onClose={() => setSnackbar(null)}
					before={
					  <Avatar size={24} style={{ background: 'var(--accent)' }}>
						  <Icon16Done fill="#fff" width={14} height={14} />
					  </Avatar>
					}
				>
					Запись опубликована
				</Snackbar>);
				if(closeModal.toString() === 'true')
					router.popPage();
		  })
		  .catch(x=>console.log(x));
	}

	function shareIM(){
		bridge.send("VKWebAppShare", {
			"link": VK_APP_URL+'#'+location.route.getLocation().split('?',1)[0] // split удаляет параметр ?m=modal_share_mobile
		  })
		  .then(_=>{
			  setSnackbar(
				  <Snackbar
					  onClose={() => setSnackbar(null)}
					  before={
						<Avatar size={24} style={{ background: 'var(--accent)' }}>
							<Icon16Done fill="#fff" width={14} height={14} />
						</Avatar>
					  }
				  >
					  Сообщение отправлено
				  </Snackbar>);
				  router.popPage();
		  })
		  .catch(x=>console.log(x));
	}

	function openPopout(){
		setContextOpened(false);
		if (isMobile) {
			router.pushModal(MODAL_SHARE_MOBILE, {tone:tone});
			console.log(location.route)
		}
		else {
			shareWall();
		}
	}

	function transpose(step, songId){
		var tone = parseInt(document.getElementById('currentToneValue').innerText);
		var newTone = tone+step;
		
		if(newTone > 11 || newTone < -11)
			newTone = 0;

		const requestOptions = {
			method: 'POST',
			headers: { 'Authorization': `Bearer ${authToken}` },
			body: new URLSearchParams({
				'songId': songId,
				'tone': newTone
			})
		};

		fetch(BASE_URL + `/songs/transpose`, requestOptions)
			.then(response => response.text())
			.then(text => {
				setSongText(text); // это работает
				setTone(newTone); // и это
				// походу tone не работает
			});
		return newTone;
	}

	return (
		<Panel id={id}>
			<PanelHeader
				left={<PanelHeaderButton onClick={()=> {
					if (isFirstPage) {
						router.replacePage(PAGE_MAIN)
					} else {
						router.popPage()
					}
				}}>
					{osName === IOS ? <Icon28ChevronBack /> : <Icon24Back />}
				</PanelHeaderButton>}
			>
				{songModel &&
					<PanelHeaderContent
						aside={<Icon16Dropdown style={{ transform: `rotate(${contextOpened ? '180deg' : '0'})` }} />}
						onClick={()=>setContextOpened(!contextOpened)}
						status={songModel.song.artist.title}>
						{songModel.song.title}
					</PanelHeaderContent>
				}
				{!songModel && 'Guitarly'}
			</PanelHeader>
			<PanelHeaderContext opened={contextOpened} onClose={()=>setContextOpened(!contextOpened)}>
				<List>
					{songModel && songModel.isPublished &&  // only if published
					<Fragment>
						<Cell
							before={<Icon28SettingsOutline />}
							onClick={()=>{
								setModalTranspose(getTransposeModal(songModel));
								router.pushModal(MODAL_TRANSPOSE);
								setContextOpened(false);
							}}
						>
							Транспонировать {tone !== 0 ? `(${['','+'][+(tone > 0)] + tone})` : ''}
						</Cell>
						<Cell
							before={isFavorite ? <Icon28UnfavoriteOutline /> : <Icon28FavoriteOutline />}
							onClick={changeFavoriteStatus}
						>
							{isFavorite ? 'Удалить из песенника' : 'Добавить в песенник'}
						</Cell>
						<Cell
							before={<Icon28ShareOutline />}
							onClick={openPopout}
						>
							Поделиться
						</Cell>
					</Fragment>}
					{!songModel && <Cell>Загрузка...</Cell>}
					{isModer &&
						<Fragment>
							{songModel && !songModel.isPublished &&
								<Cell
									before={<Icon28DoneOutline style={{color:'green'}}/>}
									onClick={publishSong}
								>
									Опубликовать
								</Cell>
							}
							<Cell
								before={<Icon28EditOutline />}
								onClick={()=>router.pushPage(PAGE_EDIT_SONG, {songId:songId})}
							>
								Редактировать
							</Cell>
							<Cell
								before={<Icon28DeleteOutline style={{color:'red'}}/>}
								onClick={deleteSong}
							>
								Удалить
							</Cell>
						</Fragment>
					}
				</List>
			</PanelHeaderContext>
			{!songText && <ScreenSpinner size='large'/>}
			{songModel &&
				<Fragment>
					<div id='currentToneValue' style={{display:'none'}}>{tone}</div>
					<Div >
						<div style={{ whiteSpace: 'pre', wordBreak: 'break-word', overflowX:'auto' }}>

							{Parser().parse(songText)}

						</div>

					</Div>
					<Separator style={{ margin: '20px 0 0 0' }} />
					<Group
						header={<Header mode="secondary">Ещё песни от артиста</Header>}>
						<SimpleCell
							before={<Avatar size={48} src={BASE_URL + songModel.song.artist.picture100} style={{ objectFit: 'cover' }} />}
							after={<Icon12ChevronOutline />}
							onClick={() => router.pushPage(PAGE_ARTIST, { artistId: songModel.song.artist.id })}>
							{songModel.song.artist.title}
						</SimpleCell>
						{songModel.isPublished && songModel.recommendations.map(song =>
							<SimpleCell
								key={'recommendation_song_' + song.id}
								after={<Icon12ChevronOutline />}
								onClick={() => router.pushPage(PAGE_SONG, {songId: song.id})}
								description={song.viewsNumber + ' ' + declOfNum(song.viewsNumber, ['просмотр', 'просмотра', 'просмотров'])}>
								{song.title}
							</SimpleCell>
						)}
					</Group>
					{songModel.isPublished && 
						<Div>
							<div id="vk_comments"></div>
						</Div>
					}
					{snackbar}
				</Fragment>}
		</Panel>
	);
}

export default Song;