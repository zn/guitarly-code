import React, { useState, useEffect, Fragment } from 'react';
import PropTypes from 'prop-types';

import { Panel, PanelHeader, PanelHeaderButton, ScreenSpinner, Spinner, Cell, Link, SimpleCell, CellButton, Div, Avatar, IconButton, HorizontalScroll, HorizontalCell, RichCell } from '@vkontakte/vkui';

import { IOS, Placeholder, platform } from '@vkontakte/vkui';
import InfiniteScroll from 'react-infinite-scroller';

import { useFirstPageCheck, useParams, useRouter } from '@happysanta/router';
import { PAGE_SONG } from '../../routers';

import { Icon12View, Icon16MoreVertical, Icon12ChevronOutline, Icon24Back, Icon28ChevronBack } from '@vkontakte/icons';

import { BASE_URL } from '../../config';

import { declOfNum } from '../../utils';

const TopSongs = ({ id }) => {

	const osName = platform();
	const router = useRouter();
	const isFirstPage = useFirstPageCheck();
	const [songs, setSongs] = useState();
	const [hasMore, setHasMore] = useState(false);

	function fetchData(page) {
		fetch(BASE_URL + '/songs/top?page=' + page)
			.then(response => response.json())
			.then(data => {
				setSongs(songs ? songs.concat(data) : data);
				if (data.length === 0) {
					setHasMore(false);
				} else {
					setHasMore(true);
				}
			});
	}

	useEffect(() => {
		fetchData(1);
	}, []);

	return (
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
				Топ песен
			</PanelHeader>
			{!songs && <ScreenSpinner />}
			{songs &&
				<Div id="scrollableDiv">
					<InfiniteScroll
						pageStart={1}
						loadMore={fetchData}
						hasMore={hasMore}
						loader={<Spinner size="small" />}
						scrollableTarget="scrollableDiv"
					>
						{songs.map((song, i) =>
							<SimpleCell
								key={'song_' + song.id}
								after={<Icon12ChevronOutline />}
								onClick={() => router.pushPage(PAGE_SONG, { songId: song.id })}
								description={song.totalViews + ' ' + declOfNum(song.totalViews, ['просмотр', 'просмотра', 'просмотров'])}>
								#{i + 1} {song.fullTitle}
							</SimpleCell>
						)}
					</InfiniteScroll>
				</Div>
			}
		</Panel>
	);
}

export default TopSongs;
